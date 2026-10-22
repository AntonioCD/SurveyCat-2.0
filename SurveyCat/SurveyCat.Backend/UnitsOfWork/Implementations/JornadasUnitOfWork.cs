using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations
{
    public class JornadasUnitOfWork : GenericUnitOfWork<Jornada>, IJornadasUnitOfWork
    {
        private readonly IJornadasRepository _jornadasRepository;

        public JornadasUnitOfWork(IGenericRepository<Jornada> repository, IJornadasRepository jornadasRepository) : base(repository)
        {
            _jornadasRepository = jornadasRepository;
        }

        public async Task<IEnumerable<Jornada>> GetComboAsync() => await _jornadasRepository.GetComboAsync();

        public override async Task<ActionResponse<IEnumerable<Jornada>>> GetAsync(PaginationDTO pagination) => await _jornadasRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _jornadasRepository.GetTotalRecordsAsync(pagination);
    }
}