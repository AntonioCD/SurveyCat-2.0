using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class PersonasRepository : GenericRepository<Persona>, IPersonasRepository
{
    private readonly DataContext _context;

    public PersonasRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Persona>> GetComboAsync()
    {
        return await _context.Personas
            .Where(p => p.TipoPersona == TipoPersona.Natural && !String.IsNullOrEmpty(p.Identificacion))
            .OrderBy(p => p.NombreCompleto)
            .ToListAsync();
    }

    public async Task<ActionResponse<IEnumerable<Persona>>> GetAsync(PersonasPaginationDTO pagination)
    {
        var queryable = _context.Personas
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        if (pagination.SoloNaturales)
        {
            queryable = queryable.Where(p => p.TipoPersona == TipoPersona.Natural);
        }

        return new ActionResponse<IEnumerable<Persona>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.NombreCompleto)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PersonasPaginationDTO pagination)
    {
        var queryable = _context.Personas
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        if (pagination.SoloNaturales)
        {
            queryable = queryable.Where(p => p.TipoPersona == TipoPersona.Natural);
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Persona>> GetAsync(long id)
    {
        var persona = await _context.Personas
             .Include(p => p.Municipio).ThenInclude(m => m!.Departamento)
             .Include(m => m.BarrioComarca)
             .Include(c => c.Caserio!)
             .Include(p => p.TipoIdentificacion)
             .Include(p => p.EstadoCivil)
             .Include(p => p.Profesion)
             .Include(p => p.TipoPersonaJuridica)
             .FirstOrDefaultAsync(m => m.Id == id);

        if (persona == null)
        {
            return new ActionResponse<Persona>
            {
                WasSuccess = false,
                Message = "Persona no existe"
            };
        }

        return new ActionResponse<Persona>
        {
            WasSuccess = true,
            Result = persona
        };
    }
}