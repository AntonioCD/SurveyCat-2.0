using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class AdjuntosUnitOfWork : GenericUnitOfWork<Adjunto>, IAdjuntosUnitOfWork
{
    private readonly IAdjuntosRepository _adjuntosRepository;

    public AdjuntosUnitOfWork(IGenericRepository<Adjunto> repository, IAdjuntosRepository adjuntosRepository) : base(repository)
    {
        _adjuntosRepository = adjuntosRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Adjunto>>> GetAsync(PaginationDTO pagination) => await _adjuntosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _adjuntosRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<Adjunto>> GetAsync(int id) => await _adjuntosRepository.GetAsync(id);
}