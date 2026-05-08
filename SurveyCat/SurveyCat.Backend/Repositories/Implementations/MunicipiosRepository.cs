using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
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

    public async Task<IEnumerable<Municipio>> GetComboAsync(int departamentoId)
    {
        return await _context.Municipios
            .Where(s => s.DepartamentoId == departamentoId)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Municipio>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Municipios
            .Include(m => m.Sectores)
            .Include(m => m.BarriosComarcas)
            .Where(x => x.Departamento!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Municipio>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Nombre)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Municipios
            .Where(x => x.Departamento!.Id == pagination.Id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public override async Task<ActionResponse<IEnumerable<Municipio>>> GetAsync()
    {
        var municipios = await _context.Municipios
            .Include(m => m.Sectores)
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
             .Include(m => m.Sectores)
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