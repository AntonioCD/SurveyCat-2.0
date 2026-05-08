using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class SectoresRepository : GenericRepository<Sector>, ISectoresRepository
{
    private readonly DataContext _context;

    public SectoresRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sector>> GetComboAsync(int municipioId)
    {
        return await _context.Sectores
            .Where(c => c.MunicipioId == municipioId)
            .OrderBy(c => c.NumeroSector)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Sector>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Sectores
            .Where(x => x.Municipio!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NumeroSector.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Sector>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.NumeroSector)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Sectores
            .Where(x => x.Municipio!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NumeroSector.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}