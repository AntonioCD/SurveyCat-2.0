using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class BarriosComarcasRepository : GenericRepository<BarrioComarca>, IBarriosComarcasRepository
{
    private readonly DataContext _context;

    public BarriosComarcasRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.BarriosComarcas
            .Include(c => c.Caserios)
            .Where(x => x.Municipio!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<BarrioComarca>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Nombre)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.BarriosComarcas
            .Where(x => x.Municipio!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public override async Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync()
    {
        var barriosComarcas = await _context.BarriosComarcas
            .Include(m => m.Caserios)
            .ToListAsync();
        return new ActionResponse<IEnumerable<BarrioComarca>>
        {
            WasSuccess = true,
            Result = barriosComarcas
        };
    }

    public override async Task<ActionResponse<BarrioComarca>> GetAsync(int id)
    {
        var barrioComarca = await _context.BarriosComarcas
             .Include(m => m.Caserios)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (barrioComarca == null)
        {
            return new ActionResponse<BarrioComarca>
            {
                WasSuccess = false,
                Message = "Barrio/Comarca no existe"
            };
        }

        return new ActionResponse<BarrioComarca>
        {
            WasSuccess = true,
            Result = barrioComarca
        };
    }
}