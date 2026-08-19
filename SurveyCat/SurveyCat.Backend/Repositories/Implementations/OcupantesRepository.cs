using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class OcupantesRepository : GenericRepository<Ocupante>, IOcupantesRepository
{
    private readonly DataContext _context;

    public OcupantesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Ocupante>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Ocupantes
            .Include(f => f.Persona!)
            .ThenInclude(f => f.TipoIdentificacion)
            .Include(f => f.TipoOcupante)
            .Include(f => f.Parentesco)
            .Where(f => f.FichaId == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Ocupante>>
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
        var queryable = _context.Ocupantes
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

    public async Task<ActionResponse<Ocupante>> GetAsync(long id)
    {
        var ocupante = await _context.Ocupantes
            .Include(p => p.Persona)
            .Include(p => p.Ficha)
            .Include(p => p.TipoOcupante)
            .Include(p => p.Parentesco)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ocupante == null)
        {
            return new ActionResponse<Ocupante>
            {
                WasSuccess = false,
                Message = "Ocupante no existe"
            };
        }

        return new ActionResponse<Ocupante>
        {
            WasSuccess = true,
            Result = ocupante
        };
    }

    public override async Task<ActionResponse<Ocupante>> AddAsync(Ocupante ocupante)
    {
        try
        {
            // 1. Buscamos cuál es el número de Item más alto asignado actualmente en esta Ficha.
            //    Usamos (int?) para evitar que falle si la ficha aún no tiene ningún miembro registrado.
            int maxItem = await _context.Set<Ocupante>()
                .Where(f => f.FichaId == ocupante.FichaId)
                .MaxAsync(f => (int?)f.Item) ?? 0;

            // 2. Asignamos el consecutivo automático (si maxItem era 0, el primero será 1)
            ocupante.Item = maxItem + 1;

            // 3. Procedemos con el guardado normal
            _context.Add(ocupante);

            await _context.SaveChangesAsync();
            return new ActionResponse<Ocupante>
            {
                WasSuccess = true,
                Result = ocupante
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Ocupante>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Ocupante>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<IEnumerable<Ocupante>>> ReorderAsync(List<Ocupante> ocupantesReordenados)
    {
        if (ocupantesReordenados == null || !ocupantesReordenados.Any())
        {
            return new ActionResponse<IEnumerable<Ocupante>>
            {
                WasSuccess = false,
                Message = "No se recibieron datos para reordenar."
            };
        }

        long fichaId = ocupantesReordenados.First().FichaId;

        try
        {
            // 1. Traemos los registros reales que están actualmente en la BD para esa Ficha
            var ocupantesBD = await _context.Ocupantes
                .Where(f => f.FichaId == fichaId)
                .ToListAsync();

            // 2. Sincronizamos los índices
            foreach (var itemUI in ocupantesReordenados)
            {
                var itemBD = ocupantesBD.FirstOrDefault(f => f.Id == itemUI.Id);
                if (itemBD != null)
                {
                    itemBD.Item = itemUI.Item;
                }
            }

            // 3. Guardamos los cambios
            await _context.SaveChangesAsync();

            return new ActionResponse<IEnumerable<Ocupante>>
            {
                WasSuccess = true,
                Result = ocupantesBD.OrderBy(f => f.Item).ToList()
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<IEnumerable<Ocupante>>
            {
                WasSuccess = false,
                Message = $"Error al persistir el reordenamiento: {exception.Message}"
            };
        }
    }

    public async Task<ActionResponse<Ocupante>> DeleteByLongAsync(long id)
    {
        var ocupanteAEliminar = await _context.Ocupantes
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ocupanteAEliminar == null)
        {
            return new ActionResponse<Ocupante>
            {
                WasSuccess = false,
                Message = "Ocupante no encontrada"
            };
        }

        try
        {
            long fichaId = ocupanteAEliminar.FichaId;

            // 1. CORRECCIÓN: Usamos el operador Value o la coalescencia (?? 0)
            // para asegurar al compilador que extraeremos un 'int' puro.
            int itemBorrado = ocupanteAEliminar.Item ?? 0;

            // 2. Eliminamos el registro elegido
            _context.Ocupantes.Remove(ocupanteAEliminar);

            // 3. CORRECCIÓN: En el Where, como f.Item es int?, extraemos su valor con .Value
            // para poder hacer la comparación numéricas limpiamente.
            var miembrosAActualizar = await _context.Ocupantes
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

            return new ActionResponse<Ocupante>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Ocupante>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}