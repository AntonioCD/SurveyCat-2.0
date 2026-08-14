using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;
using System.Linq;

namespace SurveyCat.Backend.Repositories.Implementations
{
    public class EncuestasAutorizadasRepository : GenericRepository<EncuestaAutorizada>, IEncuestasAutorizadasRepository
    {
        private readonly DataContext _context;

        public EncuestasAutorizadasRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<EncuestaAutorizada>> GetComboAsync()
        //{
        //    return await _context.EncuestasAutorizadas
        //        .Where(e => !_context.Fichas.Any(f => f.CodEncuesta == e.CodEncuesta))
        //        .OrderBy(e => e.CodEncuesta)
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<EncuestaAutorizada>> GetComboAsync()
        {
            return await _context.EncuestasAutorizadas
                .Include(e => e.Municipio)
                    .ThenInclude(m => m!.Departamento)
                .Include(e => e.BarrioComarca)
                .Include(e => e.Caserio)
                .Where(e => !_context.Fichas.Any(f => f.CodEncuesta == e.CodEncuesta))
                .OrderBy(e => e.CodEncuesta)
                .ToListAsync();
        }

        public override async Task<ActionResponse<IEnumerable<EncuestaAutorizada>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.EncuestasAutorizadas
                .Include(e => e.Municipio)
                .Include(e => e.BarrioComarca)
                .Include(e => e.Caserio)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.CodEncuesta.ToLower().Contains(pagination.Filter.ToLower()) || x.BarrioComarca!.Nombre.ToLower().Contains(pagination.Filter.ToLower()) || x.Caserio!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<EncuestaAutorizada>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.FechaCarga)
                    .ThenBy(x => x.CodEncuesta)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            var queryable = _context.EncuestasAutorizadas
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.CodEncuesta.ToLower().Contains(pagination.Filter.ToLower()) || x.BarrioComarca!.Nombre.ToLower().Contains(pagination.Filter.ToLower()) || x.Caserio!.Nombre.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<EncuestaAutorizada>> GetAsync(long id)
        {
            var encuestaAutorizada = await _context.EncuestasAutorizadas
                .Include(p => p.Municipio)
                .Include(p => p.BarrioComarca)
                .Include(p => p.Caserio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (encuestaAutorizada == null)
            {
                return new ActionResponse<EncuestaAutorizada>
                {
                    WasSuccess = false,
                    Message = "Encuesta Autorizada no existe"
                };
            }

            return new ActionResponse<EncuestaAutorizada>
            {
                WasSuccess = true,
                Result = encuestaAutorizada
            };
        }

        public async Task<ActionResponse<int>> BulkCreateAsync(List<EncuestaAutorizada> encuestas)
        {
            try
            {
                // Verificar duplicados en la base de datos
                var codigosExistente = await _context.EncuestasAutorizadas
                    .Where(e => encuestas.Select(x => x.CodEncuesta).Contains(e.CodEncuesta))
                    .Select(e => e.CodEncuesta)
                    .ToListAsync();

                if (codigosExistente.Any())
                {
                    var mensaje = $"Los siguientes códigos ya existen: {string.Join(", ", codigosExistente)}";
                    return new ActionResponse<int>
                    {
                        WasSuccess = false,
                        Message = mensaje
                    };
                }

                // Verificar duplicados dentro de la misma lista
                var duplicados = encuestas
                    .GroupBy(e => e.CodEncuesta)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicados.Any())
                {
                    var mensaje = $"Códigos duplicados en la carga: {string.Join(", ", duplicados)}";
                    return new ActionResponse<int>
                    {
                        WasSuccess = false,
                        Message = mensaje
                    };
                }

                // Validar que todas las relaciones existan
                var municipioIds = encuestas.Select(e => e.MunicipioId).Distinct().ToList();
                var barrioComarcaIds = encuestas.Select(e => e.BarrioComarcaId).Distinct().ToList();
                var caserioIds = encuestas.Where(e => e.CaserioId.HasValue).Select(e => e.CaserioId.Value).Distinct().ToList();

                // Verificar Municipios
                var municipiosExistentes = await _context.Municipios
                    .Where(m => municipioIds.Contains(m.Id))
                    .Select(m => m.Id)
                    .ToListAsync();

                var municipiosFaltantes = municipioIds.Except(municipiosExistentes).ToList();
                if (municipiosFaltantes.Any())
                {
                    return new ActionResponse<int>
                    {
                        WasSuccess = false,
                        Message = $"Los siguientes Municipios no existen: {string.Join(", ", municipiosFaltantes)}"
                    };
                }

                // Verificar Barrios/Comarcas
                var barriosExistentes = await _context.BarriosComarcas
                    .Where(b => barrioComarcaIds.Contains(b.Id))
                    .Select(b => b.Id)
                    .ToListAsync();

                var barriosFaltantes = barrioComarcaIds
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Except(barriosExistentes)
                    .ToList();

                if (barriosFaltantes.Any())
                {
                    return new ActionResponse<int>
                    {
                        WasSuccess = false,
                        Message = $"Los siguientes Barrios/Comarcas no existen: {string.Join(", ", barriosFaltantes)}"
                    };
                }

                // Verificar Caserios (si hay)
                if (caserioIds.Any())
                {
                    var caseriosExistentes = await _context.Caserios
                        .Where(c => caserioIds.Contains(c.Id))
                        .Select(c => c.Id)
                        .ToListAsync();

                    var caseriosFaltantes = caserioIds.Except(caseriosExistentes).ToList();
                    if (caseriosFaltantes.Any())
                    {
                        return new ActionResponse<int>
                        {
                            WasSuccess = false,
                            Message = $"Los siguientes Caserios no existen: {string.Join(", ", caseriosFaltantes)}"
                        };
                    }
                }

                // Asignar fechas y estado
                var fechaActual = DateTime.Now;
                foreach (var encuesta in encuestas)
                {
                    encuesta.FechaCarga = fechaActual;
                    //encuesta.FechaModificacion = fechaActual;
                    // Si tienes un campo de estado, puedes asignarlo aquí
                    // encuesta.Estado = "Activo";
                }

                // Agregar todas las encuestas
                await _context.EncuestasAutorizadas.AddRangeAsync(encuestas);
                var registrosAfectados = await _context.SaveChangesAsync();

                return new ActionResponse<int>
                {
                    WasSuccess = true,
                    Result = registrosAfectados,
                    Message = $"Se cargaron {encuestas.Count} encuestas exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = $"Error al cargar las encuestas: {ex.Message}"
                };
            }
        }
    }
}