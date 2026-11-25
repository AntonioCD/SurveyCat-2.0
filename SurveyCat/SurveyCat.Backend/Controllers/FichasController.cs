using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.Helpers;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class FichasController : GenericController<Ficha>
{
    private readonly IFichasUnitOfWork _fichasUnitOfWork;
    private readonly IFichasSyncHelper _fichasSyncHelper;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public FichasController(
        IGenericUnitOfWork<Ficha> unitOfWork,
        IFichasUnitOfWork fichasUnitOfWork,
        IFichasSyncHelper fichasSyncHelper,
        IUsersUnitOfWork usersUnitOfWork) : base(unitOfWork)
    {
        _fichasUnitOfWork = fichasUnitOfWork;
        _fichasSyncHelper = fichasSyncHelper;
        _usersUnitOfWork = usersUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _fichasUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _fichasUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _fichasUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteByLongAsync(long id)
    {
        var action = await _fichasUnitOfWork.DeleteByLongAsync(id);
        if (action.WasSuccess)
        {
            return NoContent();
        }
        return BadRequest(action.Message);
    }

    [HttpPost("sincronizar")]
    public async Task<IActionResult> SincronizarFichaCompletaAsync([FromBody] FichaSyncDTO dto)
    {
        string? userId = null;
        if (!string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            userId = currentUser?.Id;
        }

        var response = await _fichasSyncHelper.SincronizarFichaCompletaAsync(dto, userId);

        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }

        return BadRequest(response.Message);
    }
}