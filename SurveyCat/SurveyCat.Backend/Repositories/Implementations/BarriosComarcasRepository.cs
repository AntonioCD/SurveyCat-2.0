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
            .Where(x => x.Municipio!.Id == pagination.Id)
            .AsQueryable();

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

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}