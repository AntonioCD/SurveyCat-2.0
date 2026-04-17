using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;
using System.Diagnostics.Metrics;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class DepartamentosUnitOfWork : GenericUnitOfWork<Departamento>, IDepartamentosUnitOfWork
{
    private readonly IDepartamentosRepository _departamentosRepository;

    public DepartamentosUnitOfWork(IGenericRepository<Departamento> repository, IDepartamentosRepository departamentosRepository) : base(repository)
    {
        _departamentosRepository = departamentosRepository;
    }

    public async Task<IEnumerable<Departamento>> GetComboAsync() => await _departamentosRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Departamento>>> GetAsync(PaginationDTO pagination) => await _departamentosRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _departamentosRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<IEnumerable<Departamento>>> GetAsync() => await _departamentosRepository.GetAsync();

    public override async Task<ActionResponse<Departamento>> GetAsync(int id) => await _departamentosRepository.GetAsync(id);
}