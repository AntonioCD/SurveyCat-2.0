using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IDocumentosAnexosUnitOfWork
{
    Task<ActionResponse<IEnumerable<DocumentoAnexo>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<DocumentoAnexo>> GetAsync(long id);

    Task<ActionResponse<DocumentoAnexo>> DeleteByLongAsync(long id);
}