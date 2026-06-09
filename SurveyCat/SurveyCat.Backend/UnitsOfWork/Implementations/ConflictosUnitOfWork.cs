using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class ConflictosUnitOfWork : GenericUnitOfWork<Conflicto>, IConflictosUnitOfWork
{
    private readonly IConflictosRepository _conflictosRepository;

    public ConflictosUnitOfWork(IGenericRepository<Conflicto> repository, IConflictosRepository conflictosRepository) : base(repository)
    {
        _conflictosRepository = conflictosRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Conflicto>>> GetAsync(PaginationDTO pagination) => await _conflictosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _conflictosRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Conflicto>> GetAsync(long id) => await _conflictosRepository.GetAsync(id);

    public async Task<ActionResponse<Conflicto>> DeleteByLongAsync(long id) => await _conflictosRepository.DeleteByLongAsync(id);
}