using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Repositories.Implementations
{
    public class JornadasRepository : GenericRepository<Jornada>, IJornadasRepository
    {
        private readonly DataContext _context;

        public JornadasRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Jornada>> GetComboAsync()
        {
            return await _context.Jornadas
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public override async Task<ActionResponse<IEnumerable<Jornada>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Jornadas
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Jornada>>
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
            var queryable = _context.Jornadas
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
    }
}