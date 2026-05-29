using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class FamiliasUnitOfWork : GenericUnitOfWork<Familia>, IFamiliasUnitOfWork
{
    private readonly IFamiliasRepository _familiasRepository;

    public FamiliasUnitOfWork(IGenericRepository<Familia> repository, IFamiliasRepository familiasRepository) : base(repository)
    {
        _familiasRepository = familiasRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Familia>>> GetAsync(PaginationDTO pagination) => await _familiasRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _familiasRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Familia>> GetAsync(long id) => await _familiasRepository.GetAsync(id);

    public async Task<ActionResponse<Familia>> DeleteByLongAsync(long id) => await _familiasRepository.DeleteByLongAsync(id);
}