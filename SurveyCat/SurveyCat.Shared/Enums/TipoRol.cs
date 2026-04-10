using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum TipoRol
    {
        [Display(Name = "Encuestador")]
        Encuestador = 1,

        [Display(Name = "Supervisor")]
        Supervisor = 2,

        [Display(Name = "Técnico Catastral")]
        TécnicoCatastral = 3,

        [Display(Name = "Control de Calidad Legal")]
        ControlCalidadLegal = 4
    }
}