using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Implementations
{
    public class BarriosComarcasUnitOfWork : GenericUnitOfWork<BarrioComarca>, IBarriosComarcasUnitOfWork
    {
        private readonly IBarriosComarcasRepository _barriosComarcasRepository;

        public BarriosComarcasUnitOfWork(IGenericRepository<BarrioComarca> repository, IBarriosComarcasRepository barriosComarcasRepository) : base(repository)
        {
            _barriosComarcasRepository = barriosComarcasRepository;
        }

        public async Task<IEnumerable<BarrioComarca>> GetComboAsync(int municipioId) => await _barriosComarcasRepository.GetComboAsync(municipioId);

        public override async Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync(PaginationDTO pagination) => await _barriosComarcasRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _barriosComarcasRepository.GetTotalRecordsAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<BarrioComarca>>> GetAsync() => await _barriosComarcasRepository.GetAsync();

        public override async Task<ActionResponse<BarrioComarca>> GetAsync(int id) => await _barriosComarcasRepository.GetAsync(id);
    }
}