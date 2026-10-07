using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.Helpers;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class PersonasController : GenericController<Persona>
{
    private readonly IPersonasUnitOfWork _personasUnitOfWork;
    private readonly IPersonasHelper _personasHelper;

    public PersonasController(
        IGenericUnitOfWork<Persona> unitOfWork,
        IPersonasUnitOfWork personasUnitOfWork,
        IPersonasHelper personasHelper) : base(unitOfWork)
    {
        _personasUnitOfWork = personasUnitOfWork;
        _personasHelper = personasHelper;
    }

    [AllowAnonymous]
    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _personasUnitOfWork.GetComboAsync());
    }

    [HttpGet("paginatedPersonas")]
    public async Task<IActionResult> GetAsync([FromQuery] PersonasPaginationDTO pagination)
    {
        var response = await _personasUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPersonas")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PersonasPaginationDTO pagination)
    {
        var action = await _personasUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _personasUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("buscar-cedula/{cedula}")]
    public async Task<IActionResult> BuscarPorCedulaAsync(string cedula)
    {
        var response = await _personasHelper.BuscarPorCedulaAsync(cedula);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest(response.Message);
    }

    [HttpGet("buscar-por-nombre")]
    public async Task<IActionResult> BuscarPorNombreAsync(
    [FromQuery] string? primerNombre,
    [FromQuery] string? segundoNombre,
    [FromQuery] string? primerApellido,
    [FromQuery] string? segundoApellido)
    {
        var response = await _personasHelper.BuscarPorNombreAsync(primerNombre, segundoNombre, primerApellido, segundoApellido);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest(response.Message);
    }

    [HttpPost("importar-padron")]
    public async Task<IActionResult> ImportarDesdePadronAsync([FromBody] PadronPersonaDTO padronDto)
    {
        var response = await _personasHelper.ImportarDesdePadronAsync(padronDto);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest(response.Message);
    }
}