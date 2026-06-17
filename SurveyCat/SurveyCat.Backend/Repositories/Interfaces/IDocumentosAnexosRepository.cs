using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IDocumentosAnexosRepository
{
    Task<ActionResponse<IEnumerable<DocumentoAnexo>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<DocumentoAnexo>> GetAsync(long id);

    Task<ActionResponse<DocumentoAnexo>> DeleteByLongAsync(long id);
}