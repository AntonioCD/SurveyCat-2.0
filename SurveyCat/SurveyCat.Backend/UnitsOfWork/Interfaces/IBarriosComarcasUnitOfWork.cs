using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IBarriosComarcasUnitOfWork
{
    Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync();

    Task<ActionResponse<BarrioComarca>> GetAsync(int id);
}