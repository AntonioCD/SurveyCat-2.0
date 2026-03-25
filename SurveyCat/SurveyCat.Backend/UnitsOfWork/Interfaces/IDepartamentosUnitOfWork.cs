using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IDepartamentosUnitOfWork
    {
        Task<ActionResponse<IEnumerable<Departamento>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<Departamento>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Departamento>>> GetAsync();
    }
}