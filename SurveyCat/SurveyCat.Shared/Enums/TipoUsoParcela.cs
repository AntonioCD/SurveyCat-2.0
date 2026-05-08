using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum TipoUsoParcela
    {
        [Display(Name = "-- Seleccione --")]
        Seleccione = 0,

        [Display(Name = "Uso Privado")]
        Privado = 1,

        [Display(Name = "Uso Público")]
        Publico = 2
    }
}
