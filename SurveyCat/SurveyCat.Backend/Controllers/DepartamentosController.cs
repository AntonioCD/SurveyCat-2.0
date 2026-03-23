using Microsoft.AspNetCore.Mvc;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.Entities;
using System.Threading.Tasks;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : GenericController<Departamento>
{
    public DepartamentosController(IGenericUnitOfWork<Departamento> unitOfWork) : base(unitOfWork)
    {
    }
}