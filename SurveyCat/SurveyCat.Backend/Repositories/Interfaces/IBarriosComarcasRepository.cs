using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IBarriosComarcasRepository
{
    Task<IEnumerable<BarrioComarca>> GetComboAsync(int municipioId);

    Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync();

    Task<ActionResponse<BarrioComarca>> GetAsync(int id);
}