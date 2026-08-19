using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IPersonasRepository
{
    Task<IEnumerable<Persona>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Persona>>> GetAsync(PersonasPaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PersonasPaginationDTO pagination);

    Task<ActionResponse<Persona>> GetAsync(long id);
}