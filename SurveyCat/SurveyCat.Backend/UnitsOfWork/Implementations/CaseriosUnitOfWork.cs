using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class CaseriosUnitOfWork : GenericUnitOfWork<Caserio>, ICaseriosUnitOfWork
{
    private readonly ICaseriosRepository _caseriosRepository;

    public CaseriosUnitOfWork(IGenericRepository<Caserio> repository, ICaseriosRepository caseriosRepository) : base(repository)
    {
        _caseriosRepository = caseriosRepository;
    }

    public async Task<IEnumerable<Caserio>> GetComboAsync(int comarcaId) => await _caseriosRepository.GetComboAsync(comarcaId);

    public override async Task<ActionResponse<IEnumerable<Caserio>>> GetAsync(PaginationDTO pagination) => await _caseriosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _caseriosRepository.GetTotalRecordsAsync(pagination);
}