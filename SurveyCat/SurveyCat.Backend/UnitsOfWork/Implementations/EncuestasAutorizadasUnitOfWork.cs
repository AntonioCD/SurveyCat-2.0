using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class EncuestasAutorizadasUnitOfWork : GenericUnitOfWork<EncuestaAutorizada>, IEncuestasAutorizadasUnitOfWork
{
    private readonly IEncuestasAutorizadasRepository _encuestasAutorizadasRepository;

    public EncuestasAutorizadasUnitOfWork(IGenericRepository<EncuestaAutorizada> repository, IEncuestasAutorizadasRepository encuestasAutorizadasRepository) : base(repository)
    {
        _encuestasAutorizadasRepository = encuestasAutorizadasRepository;
    }

    public async Task<IEnumerable<EncuestaAutorizada>> GetComboAsync() => await _encuestasAutorizadasRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<EncuestaAutorizada>>> GetAsync(PaginationDTO pagination) => await _encuestasAutorizadasRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _encuestasAutorizadasRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<EncuestaAutorizada>> GetAsync(long id) => await _encuestasAutorizadasRepository.GetAsync(id);

    public async Task<ActionResponse<int>> BulkCreateAsync(List<EncuestaAutorizada> encuestas)
        => await _encuestasAutorizadasRepository.BulkCreateAsync(encuestas);
}