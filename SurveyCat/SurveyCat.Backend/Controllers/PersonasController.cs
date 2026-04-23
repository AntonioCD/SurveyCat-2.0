using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : GenericController<Persona>
    {
        private readonly IPersonasUnitOfWork _personasUnitOfWork;

        public PersonasController(IGenericUnitOfWork<Persona> unitOfWork, IPersonasUnitOfWork personasUnitOfWork) : base(unitOfWork)
        {
            _personasUnitOfWork = personasUnitOfWork;
        }

        [HttpGet("paginated")]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _personasUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalRecords")]
        public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
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
    }
}