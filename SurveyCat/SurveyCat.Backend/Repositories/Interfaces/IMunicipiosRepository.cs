using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IMunicipiosRepository
    {
        Task<ActionResponse<IEnumerable<Municipio>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Municipio>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Municipio>>> GetAsync();
    }
}