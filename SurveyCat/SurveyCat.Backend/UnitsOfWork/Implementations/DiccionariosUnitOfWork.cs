using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class DiccionariosUnitOfWork : GenericUnitOfWork<Diccionario>, IDiccionariosUnitOfWork
{
    private readonly IDiccionariosRepository _diccionariosRepository;

    public DiccionariosUnitOfWork(IGenericRepository<Diccionario> repository, IDiccionariosRepository diccionariosRepository) : base(repository)
    {
        _diccionariosRepository = diccionariosRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Diccionario>>> GetAsync(PaginationDTO pagination) => await _diccionariosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _diccionariosRepository.GetTotalRecordsAsync(pagination);
}