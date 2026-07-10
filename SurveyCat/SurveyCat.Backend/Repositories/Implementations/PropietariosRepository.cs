using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations;

public class PropietariosRepository : GenericRepository<Propietario>, IPropietariosRepository
{
    private readonly DataContext _context;

    public PropietariosRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Propietario>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Propietarios
            .Include(p => p.Persona!)
            .ThenInclude(p => p.TipoIdentificacion)
            .Where(p => p.FichaId == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Persona!.NombreCompleto.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Propietario>>
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
        var queryable = _context.Propietarios
            .Include(p => p.Persona!)
            .ThenInclude(p => p.TipoIdentificacion)
            .Where(p => p.FichaId == pagination.Id)
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

    public async Task<ActionResponse<Propietario>> GetAsync(long id)
    {
        var propietario = await _context.Propietarios
            .Include(p => p.Persona)
            .ThenInclude(p => p.TipoIdentificacion)
            .Include(p => p.Ficha)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (propietario == null)
        {
            return new ActionResponse<Propietario>
            {
                WasSuccess = false,
                Message = "Propietario no existe"
            };
        }

        return new ActionResponse<Propietario>
        {
            WasSuccess = true,
            Result = propietario
        };
    }

    public async Task<ActionResponse<Propietario>> DeleteByLongAsync(long id)
    {
        var propietario = await _context.Propietarios
            .FirstOrDefaultAsync(m => m.Id == id);

        if (propietario == null)
        {
            return new ActionResponse<Propietario>
            {
                WasSuccess = false,
                Message = "Propietario no encontrado"
            };
        }

        try
        {
            _context.Propietarios.Remove(propietario);
            await _context.SaveChangesAsync();

            return new ActionResponse<Propietario>
            {
                WasSuccess = true,
            };
        }
        catch
        {
            return new ActionResponse<Propietario>
            {
                WasSuccess = false,
                Message = "No se puede borrar, porque tiene registros relacionados"
            };
        }
    }
}