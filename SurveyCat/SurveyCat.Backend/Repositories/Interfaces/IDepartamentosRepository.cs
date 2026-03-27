using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IDepartamentosRepository
    {
        Task<ActionResponse<IEnumerable<Departamento>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Departamento>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Departamento>>> GetAsync();
    }
}