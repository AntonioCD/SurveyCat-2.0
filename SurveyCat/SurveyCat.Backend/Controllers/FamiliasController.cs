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
public class FamiliasController : GenericController<Familia>
{
    private readonly IFamiliasUnitOfWork _familiasUnitOfWork;

    public FamiliasController(IGenericUnitOfWork<Familia> unitOfWork, IFamiliasUnitOfWork familiasUnitOfWork) : base(unitOfWork)
    {
        _familiasUnitOfWork = familiasUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _familiasUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _familiasUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _familiasUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPost]
    public override async Task<IActionResult> PostAsync(Familia familia)
    {
        var action = await _familiasUnitOfWork.AddAsync(familia);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> ReorderAsync([FromBody] List<Familia> familiasReordenadas)
    {
        var action = await _familiasUnitOfWork.ReorderAsync(familiasReordenadas);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteByLongAsync(long id)
    {
        var action = await _familiasUnitOfWork.DeleteByLongAsync(id);
        if (action.WasSuccess)
        {
            return NoContent();
        }
        return BadRequest(action.Message);
    }
}