using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class SectoresUnitOfWork : GenericUnitOfWork<Sector>, ISectoresUnitOfWork
{
    private readonly ISectoresRepository _sectoresRepository;

    public SectoresUnitOfWork(IGenericRepository<Sector> repository, ISectoresRepository sectoresRepository) : base(repository)
    {
        _sectoresRepository = sectoresRepository;
    }

    public async Task<IEnumerable<Sector>> GetComboAsync(int municipioId) => await _sectoresRepository.GetComboAsync(municipioId);

    public override async Task<ActionResponse<IEnumerable<Sector>>> GetAsync(PaginationDTO pagination) => await _sectoresRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _sectoresRepository.GetTotalRecordsAsync(pagination);
}