using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class DocumentosAnexosRepository : GenericRepository<DocumentoAnexo>, IDocumentosAnexosRepository
{
    private readonly DataContext _context;

    public DocumentosAnexosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<DocumentoAnexo>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.DocumentosAnexos
            .Include(p => p.Documento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Documento!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<DocumentoAnexo>>
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
        var queryable = _context.DocumentosAnexos
            .Include(p => p.Documento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Documento!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<DocumentoAnexo>> GetAsync(long id)
    {
        var documentoAnexo = await _context.DocumentosAnexos
            .Include(p => p.Documento)
            .Include(p => p.Ficha)
            .Include(p => p.Adjuntos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (documentoAnexo == null)
        {
            return new ActionResponse<DocumentoAnexo>
            {
                WasSuccess = false,
                Message = "Documento Anexo no existe"
            };
        }

        return new ActionResponse<DocumentoAnexo>
        {
            WasSuccess = true,
            Result = documentoAnexo
        };
    }

    public async Task<ActionResponse<DocumentoAnexo>> DeleteByLongAsync(long id)
    {
        var documentoAnexo = await _context.DocumentosAnexos
            .FirstOrDefaultAsync(m => m.Id == id);

        if (documentoAnexo == null)
        {
            return new ActionResponse<DocumentoAnexo>
            {
                WasSuccess = false,
                Message = "Documento Anexo no encontrado"
            };
        }

        try
        {
            _context.DocumentosAnexos.Remove(documentoAnexo);
            await _context.SaveChangesAsync();

            return new ActionResponse<DocumentoAnexo>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<DocumentoAnexo>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}