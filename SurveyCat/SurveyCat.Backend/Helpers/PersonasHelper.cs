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