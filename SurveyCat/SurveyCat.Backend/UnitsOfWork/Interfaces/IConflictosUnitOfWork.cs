using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IConflictosUnitOfWork
{
    Task<ActionResponse<IEnumerable<Conflicto>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Conflicto>> GetAsync(long id);

    Task<ActionResponse<Conflicto>> DeleteByLongAsync(long id);
}