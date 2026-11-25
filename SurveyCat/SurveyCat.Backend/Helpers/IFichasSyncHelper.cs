using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.Helpers;

public interface IFichasSyncHelper
{
    Task<ActionResponse<FichaSyncResultDTO>> SincronizarFichaCompletaAsync(FichaSyncDTO dto, string? userId);
}