using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations
{
    public class PersonasUnitOfWork : GenericUnitOfWork<Persona>, IPersonasUnitOfWork
    {
        private readonly IPersonasRepository _personasRepository;

        public PersonasUnitOfWork(IGenericRepository<Persona> repository, IPersonasRepository personasRepository) : base(repository)
        {
            _personasRepository = personasRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Persona>>> GetAsync(PaginationDTO pagination) => await _personasRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _personasRepository.GetTotalRecordsAsync(pagination);

        public async Task<ActionResponse<Persona>> GetAsync(long id) => await _personasRepository.GetAsync(id);
    }
}