using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IConflictosRepository
{
    Task<ActionResponse<IEnumerable<Conflicto>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Conflicto>> GetAsync(long id);

    Task<ActionResponse<Conflicto>> DeleteByLongAsync(long id);
}