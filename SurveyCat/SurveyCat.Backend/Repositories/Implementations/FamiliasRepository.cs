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
            .Include(p => p.Persona)
            .Include(p => p.Parentesco)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Familia>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Persona!.NombreCompleto)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Familias
            .Include(p => p.Persona)
            .Include(p => p.Parentesco)
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

    public async Task<ActionResponse<Familia>> DeleteByLongAsync(long id)
    {
        var familia = await _context.Familias
            .FirstOrDefaultAsync(m => m.Id == id);

        if (familia == null)
        {
            return new ActionResponse<Familia>
            {
                WasSuccess = false,
                Message = "Familia no encontrada"
            };
        }

        try
        {
            _context.Familias.Remove(familia);
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