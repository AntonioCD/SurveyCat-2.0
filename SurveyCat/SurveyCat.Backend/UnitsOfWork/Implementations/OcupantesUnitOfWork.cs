using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class OcupantesUnitOfWork : GenericUnitOfWork<Ocupante>, IOcupantesUnitOfWork
{
    private readonly IOcupantesRepository _ocupantesRepository;

    public OcupantesUnitOfWork(IGenericRepository<Ocupante> repository, IOcupantesRepository ocupantesRepository) : base(repository)
    {
        _ocupantesRepository = ocupantesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Ocupante>>> GetAsync(PaginationDTO pagination) => await _ocupantesRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _ocupantesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Ocupante>> GetAsync(long id) => await _ocupantesRepository.GetAsync(id);

    public override async Task<ActionResponse<Ocupante>> AddAsync(Ocupante ocupante) => await _ocupantesRepository.AddAsync(ocupante);

    public async Task<ActionResponse<IEnumerable<Ocupante>>> ReorderAsync(List<Ocupante> ocupantesReordenados) => await _ocupantesRepository.ReorderAsync(ocupantesReordenados);

    public async Task<ActionResponse<Ocupante>> DeleteByLongAsync(long id) => await _ocupantesRepository.DeleteByLongAsync(id);
}