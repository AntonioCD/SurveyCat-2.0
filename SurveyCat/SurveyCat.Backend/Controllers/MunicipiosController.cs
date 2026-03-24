using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MunicipiosController : GenericController<Municipio>
{
    private readonly IMunicipiosUnitOfWork _municipiosUnitOfWork;

    public MunicipiosController(IGenericUnitOfWork<Municipio> unitOfWork, IMunicipiosUnitOfWork municipiosUnitOfWork) : base(unitOfWork)
    {
        _municipiosUnitOfWork = municipiosUnitOfWork;
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _municipiosUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _municipiosUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }
}