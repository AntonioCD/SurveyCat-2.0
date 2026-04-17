using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IDiccionariosUnitOfWork
{
    Task<IEnumerable<Diccionario>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Diccionario>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}