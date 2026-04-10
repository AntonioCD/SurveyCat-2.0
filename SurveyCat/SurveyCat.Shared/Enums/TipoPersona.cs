using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums;

public enum TipoPersona
{
    [Display(Name = "Persona Natural")]
    Natural = 1,

    [Display(Name = "Persona Jurídica")]
    Juridica = 2
}