using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class CaseriosRepository : GenericRepository<Caserio>, ICaseriosRepository
{
    private readonly DataContext _context;

    public CaseriosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Caserio>> GetComboAsync(int comarcaId)
    {
        return await _context.Caserios
            .Where(c => c.ComarcaId == comarcaId)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Caserio>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Caserios
            .Where(x => x.Comarca!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Caserio>>
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
        var queryable = _context.Caserios
            .Where(x => x.Comarca!.Id == pagination.Id)
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
}