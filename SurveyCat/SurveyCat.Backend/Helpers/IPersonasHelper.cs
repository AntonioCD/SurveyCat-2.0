using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public interface IPersonasHelper
{
    Task<ActionResponse<ResultadoBusquedaCedulaDTO>> BuscarPorCedulaAsync(string cedula);

    Task<ActionResponse<Persona>> ImportarDesdePadronAsync(PadronPersonaDTO padronDto);
}