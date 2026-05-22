using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IPropietariosRepository
{
    Task<ActionResponse<IEnumerable<Propietario>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Propietario>> GetAsync(long id);
}