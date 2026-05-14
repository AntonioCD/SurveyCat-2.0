using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IFichasUnitOfWork
    {
        Task<ActionResponse<IEnumerable<Ficha>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Ficha>> GetAsync(long id);
    }
}