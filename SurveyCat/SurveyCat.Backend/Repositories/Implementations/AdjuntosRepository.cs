using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class AdjuntosRepository : GenericRepository<Adjunto>, IAdjuntosRepository
{
    private readonly DataContext _context;

    public AdjuntosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Adjunto>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Adjuntos
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NombreArchivo.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Adjunto>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.ItemPagina)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Adjuntos
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NombreArchivo.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Adjunto>> GetAsync(long id)
    {
        var adjunto = await _context.Adjuntos
            .Include(p => p.DocumentoAnexo)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (adjunto == null)
        {
            return new ActionResponse<Adjunto>
            {
                WasSuccess = false,
                Message = "Adjunto no existe"
            };
        }

        return new ActionResponse<Adjunto>
        {
            WasSuccess = true,
            Result = adjunto
        };
    }

    public override async Task<ActionResponse<Adjunto>> AddAsync(Adjunto adjunto)
    {
        try
        {
            _context.Add(adjunto);

            await _context.SaveChangesAsync();
            return new ActionResponse<Adjunto>
            {
                WasSuccess = true,
                Result = adjunto
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Adjunto>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Adjunto>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}