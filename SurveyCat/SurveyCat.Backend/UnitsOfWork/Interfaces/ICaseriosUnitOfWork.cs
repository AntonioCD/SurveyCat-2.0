using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface ICaseriosUnitOfWork
    {
        Task<IEnumerable<Caserio>> GetComboAsync();

        Task<IEnumerable<Caserio>> GetComboAsync(int comarcaId);

        Task<ActionResponse<IEnumerable<Caserio>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}