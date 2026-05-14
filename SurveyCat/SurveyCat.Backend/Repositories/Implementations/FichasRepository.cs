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
        var persona = await _context.Fichas
             .Include(p => p.Municipio).ThenInclude(m => m!.Departamento)
             .Include(p => p.Sector)
             .Include(m => m.BarrioComarca)
             .Include(c => c.Caserio!)
             .Include(p => p.Informante)
             .Include(p => p.Encuestador)
             .Include(p => p.Coordinador)
             .Include(p => p.TecnicoCatastral)
             .Include(p => p.Estado)
             .Include(p => p.OrigenTierra)
             .Include(p => p.RelacionInformanteParcela)
             .Include(p => p.RelacionInformantePropietario)
             .Include(p => p.ServidumbreAgua)
             .Include(p => p.ServidumbrePase)
             .Include(p => p.ServidumbreOtra)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (persona == null)
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
            Result = persona
        };
    }
}