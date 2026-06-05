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
public class ColindantesController : GenericController<Colindante>
{
    private readonly IColindantesUnitOfWork _colindantesUnitOfWork;

    public ColindantesController(IGenericUnitOfWork<Colindante> unitOfWork, IColindantesUnitOfWork colindantesUnitOfWork) : base(unitOfWork)
    {
        _colindantesUnitOfWork = colindantesUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _colindantesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _colindantesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _colindantesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteByLongAsync(long id)
    {
        var action = await _colindantesUnitOfWork.DeleteByLongAsync(id);
        if (action.WasSuccess)
        {
            return NoContent();
        }
        return BadRequest(action.Message);
    }
}