using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces
{
    public interface IMunicipiosRepository
    {
        Task<ActionResponse<Municipio>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Municipio>>> GetAsync();
    }
}