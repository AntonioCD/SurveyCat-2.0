using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class DiccionariosRepository : GenericRepository<Diccionario>, IDiccionariosRepository
{
    private readonly DataContext _context;

    public DiccionariosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Diccionario>> GetComboAsync()
    {
        return await _context.Diccionarios
            .OrderBy(c => c.Catalogo)
            .ThenBy(c => c.Nombre)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Diccionario>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Diccionarios
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Catalogo.ToLower().Contains(pagination.Filter.ToLower()) || x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Diccionario>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Catalogo)
                .ThenBy(x => x.Nombre)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Diccionarios
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Catalogo.ToLower().Contains(pagination.Filter.ToLower()) || x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}