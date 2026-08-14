using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class FichasRepository : GenericRepository<Ficha>, IFichasRepository
{
    private readonly DataContext _context;

    public FichasRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Ficha>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Fichas
            .Include(f => f.Municipio)
            .Include(f => f.Sector)
            .Include(f => f.Propietarios)
            .Include(f => f.Estado)
            .Include(f => f.Ocupantes)
            .Include(f => f.Conflictos)
            .Include(f => f.DocumentosAnexos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.CodEncuesta.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Ficha>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.CodEncuesta)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Fichas
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.CodEncuesta.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Ficha>> GetAsync(long id)
    {
        var ficha = await _context.Fichas
             .Include(f => f.Municipio).ThenInclude(m => m!.Departamento)
             .Include(f => f.Sector)
             .Include(m => m.BarrioComarca)
             .Include(c => c.Caserio!)
             .Include(f => f.Informante)
             .Include(f => f.Encuestador!)
             .ThenInclude(e => e.Persona)
             .Include(f => f.Coordinador!)
             .ThenInclude(c => c.Persona)
             .Include(f => f.TecnicoCatastral!)
             .ThenInclude(t => t.Persona)
             .Include(f => f.Estado)
             .Include(f => f.UnidadMedida)
             .Include(f => f.OrigenTierra)
             .Include(f => f.RelacionInformanteParcela)
             .Include(f => f.RelacionInformantePropietario)
             .Include(f => f.ServidumbreAgua)
             .Include(f => f.ServidumbrePase)
             .Include(f => f.ServidumbreOtra)
             .Include(f => f.Propietarios)
             .Include(f => f.Ocupantes)
             .Include(f => f.Conflictos!)
             .ThenInclude(c => c.TipoConflicto)
             .Include(f => f.DocumentosAnexos)
             //.ThenInclude(f => f.Persona!)
             //.ThenInclude(f => f.TipoIdentificacion)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (ficha == null)
        {
            return new ActionResponse<Ficha>
            {
                WasSuccess = false,
                Message = "Ficha no existe"
            };
        }

        return new ActionResponse<Ficha>
        {
            WasSuccess = true,
            Result = ficha
        };
    }

    public async Task<ActionResponse<Ficha>> DeleteByLongAsync(long id)
    {
        var ficha = await _context.Fichas
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ficha == null)
        {
            return new ActionResponse<Ficha>
            {
                WasSuccess = false,
                Message = "Ficha no encontrada"
            };
        }

        try
        {
            _context.Fichas.Remove(ficha);
            await _context.SaveChangesAsync();

            return new ActionResponse<Ficha>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Ficha>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}