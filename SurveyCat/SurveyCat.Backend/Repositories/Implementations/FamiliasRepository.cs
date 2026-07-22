using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class FamiliasRepository : GenericRepository<Familia>, IFamiliasRepository
{
    private readonly DataContext _context;

    public FamiliasRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Familia>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Familias
            .Include(f => f.Persona!)
            .ThenInclude(f => f.TipoIdentificacion)
            .Include(f => f.Parentesco)
            .Where(f => f.FichaId == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Familia>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Item)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Familias
            .Include(p => p.Persona)
            .Where(f => f.FichaId == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Familia>> GetAsync(long id)
    {
        var familia = await _context.Familias
            .Include(p => p.Persona)
            .Include(p => p.Ficha)
            .Include(p => p.Parentesco)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (familia == null)
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = "Familia no existe"
            };
        }

        return new ActionResponse<Familia>
        {
            WasSuccess = true,
            Result = familia
        };
    }

    public override async Task<ActionResponse<Familia>> AddAsync(Familia familia)
    {
        try
        {
            // 1. Buscamos cuál es el número de Item más alto asignado actualmente en esta Ficha.
            //    Usamos (int?) para evitar que falle si la ficha aún no tiene ningún miembro registrado.
            int maxItem = await _context.Set<Familia>()
                .Where(f => f.FichaId == familia.FichaId)
                .MaxAsync(f => (int?)f.Item) ?? 0;

            // 2. Asignamos el consecutivo automático (si maxItem era 0, el primero será 1)
            familia.Item = maxItem + 1;

            // 3. Procedemos con el guardado normal
            _context.Add(familia);

            await _context.SaveChangesAsync();
            return new ActionResponse<Familia>
            {
                WasSuccess = true,
                Result = familia
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<IEnumerable<Familia>>> ReorderAsync(List<Familia> familiasReordenadas)
    {
        if (familiasReordenadas == null || !familiasReordenadas.Any())
        {
            return new ActionResponse<IEnumerable<Familia>>
            {
                WasSuccess = false,
                Message = "No se recibieron datos para reordenar."
            };
        }

        long fichaId = familiasReordenadas.First().FichaId;

        try
        {
            // 1. Traemos los registros reales que están actualmente en la BD para esa Ficha
            var familiasBD = await _context.Familias
                .Where(f => f.FichaId == fichaId)
                .ToListAsync();

            // 2. Sincronizamos los índices
            foreach (var itemUI in familiasReordenadas)
            {
                var itemBD = familiasBD.FirstOrDefault(f => f.Id == itemUI.Id);
                if (itemBD != null)
                {
                    itemBD.Item = itemUI.Item;
                }
            }

            // 3. Guardamos los cambios
            await _context.SaveChangesAsync();

            return new ActionResponse<IEnumerable<Familia>>
            {
                WasSuccess = true,
                Result = familiasBD.OrderBy(f => f.Item).ToList()
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<IEnumerable<Familia>>
            {
                WasSuccess = false,
                Message = $"Error al persistir el reordenamiento: {exception.Message}"
            };
        }
    }

    public async Task<ActionResponse<Familia>> DeleteByLongAsync(long id)
    {
        var familiaAEliminar = await _context.Familias
            .FirstOrDefaultAsync(m => m.Id == id);

        if (familiaAEliminar == null)
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = "Familia no encontrada"
            };
        }

        try
        {
            long fichaId = familiaAEliminar.FichaId;

            // 1. CORRECCIÓN: Usamos el operador Value o la coalescencia (?? 0)
            // para asegurar al compilador que extraeremos un 'int' puro.
            int itemBorrado = familiaAEliminar.Item ?? 0;

            // 2. Eliminamos el registro elegido
            _context.Familias.Remove(familiaAEliminar);

            // 3. CORRECCIÓN: En el Where, como f.Item es int?, extraemos su valor con .Value
            // para poder hacer la comparación numéricas limpiamente.
            var miembrosAActualizar = await _context.Familias
                .Where(f => f.FichaId == fichaId && f.Item.HasValue && f.Item.Value > itemBorrado)
                .OrderBy(f => f.Item)
                .ToListAsync();

            // 4. Les restamos 1 a su posición
            foreach (var miembro in miembrosAActualizar)
            {
                miembro.Item--;
            }

            // 5. Guardamos todo de forma atómica
            await _context.SaveChangesAsync();

            return new ActionResponse<Familia>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}