using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum TipoGenero
    {
        [Display(Name = "Femenino")]
        Femenino = 1,

        [Display(Name = "Masculino")]
        Masculino = 2
    }
}