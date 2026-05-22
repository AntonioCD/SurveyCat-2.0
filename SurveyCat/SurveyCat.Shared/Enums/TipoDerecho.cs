using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum TipoDerecho
    {
        [Display(Name = "-- Seleccione --")]
        Seleccione = 0,

        [Display(Name = "Propietario")]
        Propietario = 1,

        [Display(Name = "Poseedor")]
        Poseedor = 2
    }
}