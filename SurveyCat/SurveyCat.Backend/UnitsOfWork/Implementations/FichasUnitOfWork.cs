using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations
{
    public class FichasUnitOfWork : GenericUnitOfWork<Ficha>, IFichasUnitOfWork
    {
        private readonly IFichasRepository _fichasRepository;

        public FichasUnitOfWork(IGenericRepository<Ficha> repository, IFichasRepository fichasRepository) : base(repository)
        {
            _fichasRepository = fichasRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Ficha>>> GetAsync(PaginationDTO pagination) => await _fichasRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _fichasRepository.GetTotalRecordsAsync(pagination);

        public async Task<ActionResponse<Ficha>> GetAsync(long id) => await _fichasRepository.GetAsync(id);
    }
}