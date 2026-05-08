using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface ISectoresUnitOfWork
    {
        Task<IEnumerable<Sector>> GetComboAsync(int municipioId);

        Task<ActionResponse<IEnumerable<Sector>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}