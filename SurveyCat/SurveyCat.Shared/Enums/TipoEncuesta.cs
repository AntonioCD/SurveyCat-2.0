using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums;

public enum TipoEncuesta
{
    [Display(Name = "-- Seleccione --")]
    Seleccione = 0,

    [Display(Name = "Parcela Unificada")]
    Unificada = 1,

    [Display(Name = "Parcela Horizontal")]
    Horizontal = 2
}
