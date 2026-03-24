using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces
{
    public interface IMunicipiosUnitOfWork
    {
        Task<ActionResponse<Municipio>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Municipio>>> GetAsync();
    }
}