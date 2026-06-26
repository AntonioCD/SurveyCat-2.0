using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IAdjuntosUnitOfWork
{
    Task<ActionResponse<IEnumerable<Adjunto>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Adjunto>> GetAsync(int id);

    Task<ActionResponse<Adjunto>> AddAsync(Adjunto adjunto);
}