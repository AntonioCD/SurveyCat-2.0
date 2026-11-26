using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public interface IFichasSyncHelper
{
    Task<ActionResponse<List<FichaEstadoResponseDto>>> ConsultarEstadosAsync(List<string> codEncuestas);

    Task<ActionResponse<FichaSyncResultDTO>> SincronizarFichaCompletaAsync(FichaSyncDTO dto, string? userId);
}