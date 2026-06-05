using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class ColindantesUnitOfWork : GenericUnitOfWork<Colindante>, IColindantesUnitOfWork
{
    private readonly IColindantesRepository _colindantesRepository;

    public ColindantesUnitOfWork(IGenericRepository<Colindante> repository, IColindantesRepository colindantesRepository) : base(repository)
    {
        _colindantesRepository = colindantesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Colindante>>> GetAsync(PaginationDTO pagination) => await _colindantesRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _colindantesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Colindante>> GetAsync(long id) => await _colindantesRepository.GetAsync(id);

    public async Task<ActionResponse<Colindante>> DeleteByLongAsync(long id) => await _colindantesRepository.DeleteByLongAsync(id);
}