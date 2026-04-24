using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class PersonalEncuestasUnitOfWork : GenericUnitOfWork<PersonalEncuesta>, IPersonalEncuestasUnitOfWork
{
    private readonly IPersonalEncuestasRepository _personalEncuestasRepository;

    public PersonalEncuestasUnitOfWork(IGenericRepository<PersonalEncuesta> repository, IPersonalEncuestasRepository personalEncuestasRepository) : base(repository)
    {
        _personalEncuestasRepository = personalEncuestasRepository;
    }

    public override async Task<ActionResponse<IEnumerable<PersonalEncuesta>>> GetAsync(PaginationDTO pagination) => await _personalEncuestasRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _personalEncuestasRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<PersonalEncuesta>> GetAsync(int id) => await _personalEncuestasRepository.GetAsync(id);
}