using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using System.Threading.Tasks;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : GenericController<Departamento>
{
    private readonly IDepartamentosUnitOfWork _departamentosUnitOfWork;

    public DepartamentosController(IGenericUnitOfWork<Departamento> unitOfWork, IDepartamentosUnitOfWork departamentosUnitOfWork) : base(unitOfWork)
    {
        _departamentosUnitOfWork = departamentosUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _departamentosUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _departamentosUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _departamentosUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }
}