using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class EncuestasAutorizadasController : GenericController<EncuestaAutorizada>
{
    private readonly IEncuestasAutorizadasUnitOfWork _encuestasAutorizadasUnitOfWork;

    public EncuestasAutorizadasController(IGenericUnitOfWork<EncuestaAutorizada> unitOfWork, IEncuestasAutorizadasUnitOfWork encuestasAutorizadasUnitOfWork) : base(unitOfWork)
    {
        _encuestasAutorizadasUnitOfWork = encuestasAutorizadasUnitOfWork;
    }

    [AllowAnonymous]
    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _encuestasAutorizadasUnitOfWork.GetComboAsync());
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _encuestasAutorizadasUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _encuestasAutorizadasUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var response = await _encuestasAutorizadasUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] List<EncuestaAutorizada> encuestas)
    {
        if (encuestas == null || !encuestas.Any())
        {
            return BadRequest(new { Message = "No hay encuestas para procesar" });
        }

        // Validar que no exceda el límite máximo
        if (encuestas.Count > 500)
        {
            return BadRequest(new { Message = "No se pueden procesar más de 500 encuestas por carga" });
        }

        var response = await _encuestasAutorizadasUnitOfWork.BulkCreateAsync(encuestas);

        if (response.WasSuccess)
        {
            // Devolver un objeto con los datos relevantes
            return Ok(new
            {
                success = true,
                total = encuestas.Count,
                message = $"Se cargaron {encuestas.Count} encuestas exitosamente"
            });
        }

        return BadRequest(new { Message = response.Message });
    }

    //[HttpPost("bulk")]
    //public async Task<IActionResult> BulkCreate(List<EncuestaAutorizada> encuestas)
    //{
    //    if (encuestas == null || !encuestas.Any())
    //    {
    //        return BadRequest("No hay encuestas para procesar");
    //    }

    //    // Validar que no exceda el límite máximo
    //    if (encuestas.Count > 500)
    //    {
    //        return BadRequest("No se pueden procesar más de 500 encuestas por carga");
    //    }

    //    var response = await _encuestasAutorizadasUnitOfWork.BulkCreateAsync(encuestas);

    //    if (response.WasSuccess)
    //    {
    //        return Ok(response.Result);
    //    }

    //    return BadRequest(response.Message);
    //}
}