using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarriosComarcasController : GenericController<BarrioComarca>
{
    private readonly IBarriosComarcasUnitOfWork _barriosComarcasUnitOfWork;

    public BarriosComarcasController(IGenericUnitOfWork<BarrioComarca> unitOfWork, IBarriosComarcasUnitOfWork barriosComarcasUnitOfWork) : base(unitOfWork)
    {
        _barriosComarcasUnitOfWork = barriosComarcasUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _barriosComarcasUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _barriosComarcasUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }
}