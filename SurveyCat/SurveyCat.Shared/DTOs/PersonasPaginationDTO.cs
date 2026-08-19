using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs;

public class PersonasPaginationDTO : PaginationDTO
{
    public bool SoloNaturales { get; set; } = false;
}