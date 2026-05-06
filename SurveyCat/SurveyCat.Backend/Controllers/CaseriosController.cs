using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class CaseriosController : GenericController<Caserio>
{
    private readonly ICaseriosUnitOfWork _caseriosUnitOfWork;

    public CaseriosController(IGenericUnitOfWork<Caserio> unitOfWork, ICaseriosUnitOfWork caseriosUnitOfWork) : base(unitOfWork)
    {
        _caseriosUnitOfWork = caseriosUnitOfWork;
    }

    [AllowAnonymous]
    [HttpGet("combo/{comarcaId:int}")]
    public async Task<IActionResult> GetComboAsync(int comarcaId)
    {
        return Ok(await _caseriosUnitOfWork.GetComboAsync(comarcaId));
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _caseriosUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _caseriosUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }
}