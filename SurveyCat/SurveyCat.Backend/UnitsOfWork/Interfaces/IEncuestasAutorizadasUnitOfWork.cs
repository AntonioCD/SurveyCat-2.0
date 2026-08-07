using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IEncuestasAutorizadasUnitOfWork
    {
        Task<IEnumerable<EncuestaAutorizada>> GetComboAsync();

        Task<ActionResponse<IEnumerable<EncuestaAutorizada>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<EncuestaAutorizada>> GetAsync(long id);

        Task<ActionResponse<int>> BulkCreateAsync(List<EncuestaAutorizada> encuestas);
    }
}