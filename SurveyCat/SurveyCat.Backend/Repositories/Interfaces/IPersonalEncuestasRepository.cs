using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IPersonalEncuestasRepository
{
    Task<IEnumerable<PersonalEncuesta>> GetComboAsync();

    Task<ActionResponse<IEnumerable<PersonalEncuesta>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<PersonalEncuesta>> GetAsync(int id);
}