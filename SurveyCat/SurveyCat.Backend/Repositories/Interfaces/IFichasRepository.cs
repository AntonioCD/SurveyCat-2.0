using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;
using System.Threading.Tasks;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IFichasRepository
    {
        Task<ActionResponse<IEnumerable<Ficha>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<ActionResponse<Ficha>> GetAsync(long id);
    }
}