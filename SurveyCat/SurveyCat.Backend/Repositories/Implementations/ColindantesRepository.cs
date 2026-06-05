using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class ColindantesRepository : GenericRepository<Colindante>, IColindantesRepository
{
    private readonly DataContext _context;

    public ColindantesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Colindante>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Colindantes
            .Include(p => p.Persona)
            .ThenInclude(p => p.TipoIdentificacion)
            .Include(p => p.PuntoCardinal)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Colindante>>
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
        var queryable = _context.Colindantes
            .Include(p => p.Persona)
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

    public async Task<ActionResponse<Colindante>> GetAsync(long id)
    {
        var colindante = await _context.Colindantes
            .Include(p => p.Persona)
            .Include(p => p.Ficha)
            .Include(p => p.PuntoCardinal)
            .Include(p => p.Conflicto)
            .Include(p => p.ViaGestion)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (colindante == null)
        {
            return new ActionResponse<Colindante>
            {
                WasSuccess = false,
                Message = "Colindante no existe"
            };
        }

        return new ActionResponse<Colindante>
        {
            WasSuccess = true,
            Result = colindante
        };
    }

    public async Task<ActionResponse<Colindante>> DeleteByLongAsync(long id)
    {
        var colindante = await _context.Colindantes
            .FirstOrDefaultAsync(m => m.Id == id);

        if (colindante == null)
        {
            return new ActionResponse<Colindante>
            {
                WasSuccess = false,
                Message = "Colindante no encontrado"
            };
        }

        try
        {
            _context.Colindantes.Remove(colindante);
            await _context.SaveChangesAsync();

            return new ActionResponse<Colindante>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Colindante>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}