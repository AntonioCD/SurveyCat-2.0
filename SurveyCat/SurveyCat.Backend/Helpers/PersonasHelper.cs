using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public class PersonasHelper : IPersonasHelper
{
    private readonly DataContext _context;

    public PersonasHelper(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResponse<ResultadoBusquedaCedulaDTO>> BuscarPorCedulaAsync(string cedula)
    {
        try
        {
            // Limpieza de caracteres para la comparación (sin guiones ni espacios)
            string cedulaLimpia = cedula.Replace("-", "").Replace(" ", "").Trim().ToUpper();

            var resultado = new ResultadoBusquedaCedulaDTO();

            // 1. Buscar en la tabla Persona del sistema
            var personaSistema = await _context.Personas
                .Include(p => p.Municipio).ThenInclude(m => m!.Departamento)
                .Include(p => p.BarrioComarca)
                .Include(p => p.Caserio)
                .Include(p => p.TipoIdentificacion)
                .Include(p => p.EstadoCivil)
                .Include(p => p.Profesion)
                .FirstOrDefaultAsync(p => p.Identificacion != null &&
                                          p.Identificacion.Replace("-", "").Replace(" ", "").Trim().ToUpper() == cedulaLimpia);

            if (personaSistema != null)
            {
                resultado.EncontradoEnSistema = true;
                resultado.PersonaExistente = personaSistema;

                return new ActionResponse<ResultadoBusquedaCedulaDTO>
                {
                    WasSuccess = true,
                    Result = resultado
                };
            }

            // 2. Si no existe en el sistema, buscar en la tabla Padron
            var padronRecord = await _context.Padron
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Cedula.Replace("-", "").Replace(" ", "").Trim().ToUpper() == cedulaLimpia);

            if (padronRecord != null)
            {
                resultado.EncontradoEnPadron = true;
                resultado.DatosPadron = new PadronPersonaDTO
                {
                    Id = padronRecord.Id,
                    Cedula = padronRecord.Cedula,
                    PrimerNombre = padronRecord.Primer_Nombre,
                    SegundoNombre = padronRecord.Segundo_Nombre,
                    PrimerApellido = padronRecord.Primer_Apellido,
                    SegundoApellido = padronRecord.Segundo_Apellido,
                    Sexo = padronRecord.Sexo,
                    Domicilio = padronRecord.Domicilio
                };

                return new ActionResponse<ResultadoBusquedaCedulaDTO>
                {
                    WasSuccess = true,
                    Result = resultado
                };
            }

            // 3. Ni en sistema ni en padrón
            return new ActionResponse<ResultadoBusquedaCedulaDTO>
            {
                WasSuccess = true,
                Result = resultado
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<ResultadoBusquedaCedulaDTO>
            {
                WasSuccess = false,
                Message = $"Error al consultar la cédula: {ex.Message}"
            };
        }
    }

    public async Task<ActionResponse<ResultadoBusquedaNombreDTO>> BuscarPorNombreAsync(
     string? primerNombre,
     string? segundoNombre,
     string? primerApellido,
     string? segundoApellido)
    {
        if (string.IsNullOrWhiteSpace(primerNombre) &&
            string.IsNullOrWhiteSpace(segundoNombre) &&
            string.IsNullOrWhiteSpace(primerApellido) &&
            string.IsNullOrWhiteSpace(segundoApellido))
        {
            return new ActionResponse<ResultadoBusquedaNombreDTO>
            {
                WasSuccess = false,
                Message = "Debe ingresar al menos un nombre o apellido para realizar la búsqueda."
            };
        }

        var pNombreUpper = primerNombre?.Trim().ToUpper() ?? string.Empty;
        var sNombreUpper = segundoNombre?.Trim().ToUpper() ?? string.Empty;
        var pApellidoUpper = primerApellido?.Trim().ToUpper() ?? string.Empty;
        var sApellidoUpper = segundoApellido?.Trim().ToUpper() ?? string.Empty;

        var resultado = new ResultadoBusquedaNombreDTO();

        // 1. Búsqueda en la tabla local 'Persona'
        var queryPersona = _context.Personas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(pNombreUpper))
        {
            queryPersona = queryPersona.Where(p => (p.PrimerNombre != null && p.PrimerNombre.ToUpper().StartsWith(pNombreUpper)) || p.NombreCompleto.ToUpper().Contains(pNombreUpper));
        }
        if (!string.IsNullOrEmpty(sNombreUpper))
        {
            queryPersona = queryPersona.Where(p => (p.SegundoNombre != null && p.SegundoNombre.ToUpper().StartsWith(sNombreUpper)) || p.NombreCompleto.ToUpper().Contains(sNombreUpper));
        }
        if (!string.IsNullOrEmpty(pApellidoUpper))
        {
            queryPersona = queryPersona.Where(p => (p.PrimerApellido != null && p.PrimerApellido.ToUpper().StartsWith(pApellidoUpper)) || p.NombreCompleto.ToUpper().Contains(pApellidoUpper));
        }
        if (!string.IsNullOrEmpty(sApellidoUpper))
        {
            queryPersona = queryPersona.Where(p => (p.SegundoApellido != null && p.SegundoApellido.ToUpper().StartsWith(sApellidoUpper)) || p.NombreCompleto.ToUpper().Contains(sApellidoUpper));
        }

        resultado.PersonasEnSistema = await queryPersona
            .Take(20)
            .ToListAsync();

        // 2. Búsqueda en 'Padron' (4 Millones de registros - Uso eficiente del índice)
        var queryPadron = _context.Padron.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(pApellidoUpper))
        {
            queryPadron = queryPadron.Where(p => p.Primer_Apellido.StartsWith(pApellidoUpper));
        }
        if (!string.IsNullOrEmpty(sApellidoUpper))
        {
            queryPadron = queryPadron.Where(p => p.Segundo_Apellido != null && p.Segundo_Apellido.StartsWith(sApellidoUpper));
        }
        if (!string.IsNullOrEmpty(pNombreUpper))
        {
            queryPadron = queryPadron.Where(p => p.Primer_Nombre.StartsWith(pNombreUpper));
        }
        if (!string.IsNullOrEmpty(sNombreUpper))
        {
            queryPadron = queryPadron.Where(p => p.Segundo_Nombre != null && p.Segundo_Nombre.StartsWith(sNombreUpper));
        }

        var cedulasExistentes = resultado.PersonasEnSistema
            .Where(p => !string.IsNullOrEmpty(p.Identificacion))
            .Select(p => p.Identificacion!)
            .ToList();

        var padronList = await queryPadron
            .Where(p => !cedulasExistentes.Contains(p.Cedula))
            .Take(20)
            .Select(p => new PadronPersonaDTO
            {
                Cedula = p.Cedula,
                PrimerNombre = p.Primer_Nombre,
                SegundoNombre = p.Segundo_Nombre,
                PrimerApellido = p.Primer_Apellido,
                SegundoApellido = p.Segundo_Apellido,
                Sexo = p.Sexo,
                Domicilio = p.Domicilio
            })
            .ToListAsync();

        resultado.PersonasEnPadron = padronList;

        return new ActionResponse<ResultadoBusquedaNombreDTO>
        {
            WasSuccess = true,
            Result = resultado
        };
    }

    public async Task<ActionResponse<Persona>> ImportarDesdePadronAsync(PadronPersonaDTO padronDto)
    {
        try
        {
            var nuevaPersona = new Persona
            {
                Identificacion = padronDto.Cedula,
                PrimerNombre = padronDto.PrimerNombre,
                SegundoNombre = padronDto.SegundoNombre,
                PrimerApellido = padronDto.PrimerApellido,
                SegundoApellido = padronDto.SegundoApellido,
                Genero = padronDto.Sexo?.Trim().ToUpper() == "M" ? TipoGenero.Masculino : TipoGenero.Femenino,
                Direccion = padronDto.Domicilio,
                TipoPersona = TipoPersona.Natural
            };

            _context.Personas.Add(nuevaPersona);
            await _context.SaveChangesAsync();

            return new ActionResponse<Persona>
            {
                WasSuccess = true,
                Result = nuevaPersona
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Persona>
            {
                WasSuccess = false,
                Message = $"Error al importar la persona desde el Padrón: {ex.Message}"
            };
        }
    }
}