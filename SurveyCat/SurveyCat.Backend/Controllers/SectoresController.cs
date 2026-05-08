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
public class SectoresController : GenericController<Sector>
{
    private readonly ISectoresUnitOfWork _sectoresUnitOfWork;

    public SectoresController(IGenericUnitOfWork<Sector> unitOfWork, ISectoresUnitOfWork sectoresUnitOfWork) : base(unitOfWork)
    {
        _sectoresUnitOfWork = sectoresUnitOfWork;
    }

    [AllowAnonymous]
    [HttpGet("combo/{municipioId:int}")]
    public async Task<IActionResult> GetComboAsync(int municipioId)
    {
        return Ok(await _sectoresUnitOfWork.GetComboAsync(municipioId));
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _sectoresUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _sectoresUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }
}