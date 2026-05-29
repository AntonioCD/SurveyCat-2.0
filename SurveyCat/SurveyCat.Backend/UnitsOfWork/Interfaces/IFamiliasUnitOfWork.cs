using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IFamiliasUnitOfWork
    {
        Task<ActionResponse<IEnumerable<Familia>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Familia>> GetAsync(long id);

        Task<ActionResponse<Familia>> DeleteByLongAsync(long id);
    }
}