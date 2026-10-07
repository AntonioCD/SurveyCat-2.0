using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public interface IPersonasHelper
{
    Task<ActionResponse<ResultadoBusquedaCedulaDTO>> BuscarPorCedulaAsync(string cedula);

    Task<ActionResponse<Persona>> ImportarDesdePadronAsync(PadronPersonaDTO padronDto);

    Task<ActionResponse<ResultadoBusquedaNombreDTO>> BuscarPorNombreAsync(
        string? primerNombre,
        string? segundoNombre,
        string? primerApellido,
        string? segundoApellido);
}