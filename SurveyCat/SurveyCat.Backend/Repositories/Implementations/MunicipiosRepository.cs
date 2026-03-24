using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class MunicipiosRepository : GenericRepository<Municipio>, IMunicipiosRepository
{
    private readonly DataContext _context;

    public MunicipiosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Municipio>>> GetAsync()
    {
        var municipios = await _context.Municipios
            .Include(m => m.BarriosComarcas)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Municipio>>
        {
            WasSuccess = true,
            Result = municipios
        };
    }

    public override async Task<ActionResponse<Municipio>> GetAsync(int id)
    {
        var municipio = await _context.Municipios
             .Include(m => m.BarriosComarcas!)
             .ThenInclude(c => c.Caserios!)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (municipio == null)
        {
            return new ActionResponse<Municipio>
            {
                WasSuccess = false,
                Message = "Municipio no existe"
            };
        }

        return new ActionResponse<Municipio>
        {
            WasSuccess = true,
            Result = municipio
        };
    }
}