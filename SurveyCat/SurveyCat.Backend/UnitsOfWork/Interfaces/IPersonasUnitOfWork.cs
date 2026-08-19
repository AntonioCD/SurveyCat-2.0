using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IPersonasUnitOfWork
    {
        Task<IEnumerable<Persona>> GetComboAsync();

        Task<ActionResponse<IEnumerable<Persona>>> GetAsync(PersonasPaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PersonasPaginationDTO pagination);

        Task<ActionResponse<Persona>> GetAsync(long id);
    }
}