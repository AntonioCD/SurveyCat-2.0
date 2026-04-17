using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class MunicipiosUnitOfWork : GenericUnitOfWork<Municipio>, IMunicipiosUnitOfWork
{
    private readonly IMunicipiosRepository _municipiosRepository;

    public MunicipiosUnitOfWork(IGenericRepository<Municipio> repository, IMunicipiosRepository municipiosRepository) : base(repository)
    {
        _municipiosRepository = municipiosRepository;
    }

    public async Task<IEnumerable<Municipio>> GetComboAsync(int departamentoId) => await _municipiosRepository.GetComboAsync(departamentoId);

    public override async Task<ActionResponse<IEnumerable<Municipio>>> GetAsync(PaginationDTO pagination) => await _municipiosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _municipiosRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<IEnumerable<Municipio>>> GetAsync() => await _municipiosRepository.GetAsync();

    public override async Task<ActionResponse<Municipio>> GetAsync(int id) => await _municipiosRepository.GetAsync(id);
}