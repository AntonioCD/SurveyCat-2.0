using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.Data;
using SurveyCat.Shared.Entities;
using System.Threading.Tasks;

namespace SurveyCat.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentosController : ControllerBase
    {
        private readonly DataContext _context;

        public DepartamentosController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Departamento departamento)
        {
            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();
            return Ok(departamento);
        }
    }
}