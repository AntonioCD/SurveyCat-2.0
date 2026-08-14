using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class OcupantesController : GenericController<Ocupante>
{
    private readonly IOcupantesUnitOfWork _ocupantesUnitOfWork;

    public OcupantesController(IGenericUnitOfWork<Ocupante> unitOfWork, IOcupantesUnitOfWork ocupantesUnitOfWork) : base(unitOfWork)
    {
        _ocupantesUnitOfWork = ocupantesUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _ocupantesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _ocupantesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _ocupantesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPost]
    public override async Task<IActionResult> PostAsync(Ocupante ocupante)
    {
        var action = await _ocupantesUnitOfWork.AddAsync(ocupante);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> ReorderAsync([FromBody] List<Ocupante> ocupantesReordenados)
    {
        var action = await _ocupantesUnitOfWork.ReorderAsync(ocupantesReordenados);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteByLongAsync(long id)
    {
        var action = await _ocupantesUnitOfWork.DeleteByLongAsync(id);
        if (action.WasSuccess)
        {
            return NoContent();
        }
        return BadRequest(action.Message);
    }
}