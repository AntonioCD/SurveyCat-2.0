using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Diccionario
{
    public int Id { get; set; }

    [Display(Name = "Catálogo")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "El campo {0} no puede contener espacios.")]
    public string Catalogo { get; set; } = null!;

    [Display(Name = "Nombre")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Descripción")]
    [MaxLength(256, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Código")]
    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? Codigo { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}