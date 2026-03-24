using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class DepartamentosRepository : GenericRepository<Departamento>, IDepartamentosRepository
{
    private readonly DataContext _context;

    public DepartamentosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Departamento>>> GetAsync()
    {
        var departamentos = await _context.Departamentos
            .Include(c => c.Municipios)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Departamento>>
        {
            WasSuccess = true,
            Result = departamentos
        };
    }

    public override async Task<ActionResponse<Departamento>> GetAsync(int id)
    {
        var departamento = await _context.Departamentos
             .Include(d => d.Municipios!)
             .ThenInclude(m => m.BarriosComarcas!)
             .ThenInclude(c => c.Caserios)
             .FirstOrDefaultAsync(d => d.Id == id);

        if (departamento == null)
        {
            return new ActionResponse<Departamento>
            {
                WasSuccess = false,
                Message = "Departamento no existe"
            };
        }

        return new ActionResponse<Departamento>
        {
            WasSuccess = true,
            Result = departamento
        };
    }
}