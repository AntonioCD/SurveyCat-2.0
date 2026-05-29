using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IFamiliasRepository
    {
        Task<ActionResponse<IEnumerable<Familia>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Familia>> GetAsync(long id);

        Task<ActionResponse<Familia>> DeleteByLongAsync(long id);
    }
}