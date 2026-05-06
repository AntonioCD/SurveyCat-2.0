using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class PersonalEncuestasRepository : GenericRepository<PersonalEncuesta>, IPersonalEncuestasRepository
{
    private readonly DataContext _context;

    public PersonalEncuestasRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PersonalEncuesta>> GetComboAsync()
    {
        return await _context.PersonalEncuestas
            .Include(m => m.Persona)
            .OrderBy(c => c.Persona!.NombreCompleto)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<PersonalEncuesta>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.PersonalEncuestas
            .Include(m => m.Persona)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<PersonalEncuesta>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Persona!.NombreCompleto)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.PersonalEncuestas
            .Include(m => m.Persona)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public override async Task<ActionResponse<PersonalEncuesta>> GetAsync(int id)
    {
        var personalEncuesta = await _context.PersonalEncuestas
             .Include(m => m.Persona)
             .Include(c => c.User)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (personalEncuesta == null)
        {
            return new ActionResponse<PersonalEncuesta>
            {
                WasSuccess = false,
                Message = "Personal de Encuesta no existe"
            };
        }

        return new ActionResponse<PersonalEncuesta>
        {
            WasSuccess = true,
            Result = personalEncuesta
        };
    }
}