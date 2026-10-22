using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IJornadasRepository
    {
        Task<IEnumerable<Jornada>> GetComboAsync();

        Task<ActionResponse<IEnumerable<Jornada>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}