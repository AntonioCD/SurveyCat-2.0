using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    [HttpGet("combo/{municipioId:int}")]
    public async Task<IActionResult> GetComboAsync(int municipioId)
    {
        return Ok(await _barriosComarcasUnitOfWork.GetComboAsync(municipioId));
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

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _barriosComarcasUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _barriosComarcasUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }
}