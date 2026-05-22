using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class PropietariosUnitOfWork : GenericUnitOfWork<Propietario>, IPropietariosUnitOfWork
{
    private readonly IPropietariosRepository _propietariosRepository;

    public PropietariosUnitOfWork(IGenericRepository<Propietario> repository, IPropietariosRepository propietariosRepository) : base(repository)
    {
        _propietariosRepository = propietariosRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Propietario>>> GetAsync(PaginationDTO pagination) => await _propietariosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _propietariosRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Propietario>> GetAsync(long id) => await _propietariosRepository.GetAsync(id);
}