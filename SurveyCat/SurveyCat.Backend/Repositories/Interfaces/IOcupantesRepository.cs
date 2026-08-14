using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IOcupantesRepository
{
    Task<ActionResponse<IEnumerable<Ocupante>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Ocupante>> GetAsync(long id);

    Task<ActionResponse<Ocupante>> AddAsync(Ocupante ocupante);

    Task<ActionResponse<IEnumerable<Ocupante>>> ReorderAsync(List<Ocupante> ocupantesReordenados);

    Task<ActionResponse<Ocupante>> DeleteByLongAsync(long id);
}