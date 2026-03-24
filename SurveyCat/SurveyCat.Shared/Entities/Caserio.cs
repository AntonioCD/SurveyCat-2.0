using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Caserio
{
    public int Id { get; set; }

    [Display(Name = "Descripción")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Código Caserío")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a 0.")]
    public int CodCaserio { get; set; }

    public int ComarcaId { get; set; }
    public BarrioComarca Comarca { get; set; } = null!;
}