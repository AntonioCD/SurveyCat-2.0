using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums;

public enum TipoSector
{
    [Display(Name = "Sector Urbano")]
    Urbano = 1,

    [Display(Name = "Sector Rural")]
    Rural = 2
}