using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class ConflictosRepository : GenericRepository<Conflicto>, IConflictosRepository
{
    private readonly DataContext _context;

    public ConflictosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Conflicto>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Conflictos
            .Include(p => p.TipoConflicto)
            .Include(p => p.ViaGestion)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.TipoConflicto!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Conflicto>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Id)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Conflictos
            .Include(p => p.TipoConflicto)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.TipoConflicto!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Conflicto>> GetAsync(long id)
    {
        var conflicto = await _context.Conflictos
            .Include(p => p.TipoConflicto)
            .Include(p => p.ViaGestion)
            .Include(p => p.Ficha)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (conflicto == null)
        {
            return new ActionResponse<Conflicto>
            {
                WasSuccess = false,
                Message = "Conflicto no existe"
            };
        }

        return new ActionResponse<Conflicto>
        {
            WasSuccess = true,
            Result = conflicto
        };
    }

    public async Task<ActionResponse<Conflicto>> DeleteByLongAsync(long id)
    {
        var conflicto = await _context.Conflictos
            .FirstOrDefaultAsync(m => m.Id == id);

        if (conflicto == null)
        {
            return new ActionResponse<Conflicto>
            {
                WasSuccess = false,
                Message = "Conflicto no encontrado"
            };
        }

        try
        {
            _context.Conflictos.Remove(conflicto);
            await _context.SaveChangesAsync();

            return new ActionResponse<Conflicto>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Conflicto>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}