using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum PerfilPropietario
    {
        [Display(Name = "-- Seleccione --")]
        Seleccione = 0,

        [Display(Name = "Desmovilizado")]
        Desmovilizado = 1,

        [Display(Name = "Retirado")]
        Retirado = 2,

        [Display(Name = "Campesino Tradicional")]
        CampesinoTradicional = 3,

        [Display(Name = "Agricultor")]
        Agricultor = 4,

        [Display(Name = "Ganadero")]
        Ganadero = 5,

        [Display(Name = "Agropecuario")]
        Agropecuario = 6,

        [Display(Name = "Otro")]
        Otro = 7
    }
}