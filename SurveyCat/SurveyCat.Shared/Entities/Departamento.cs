using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Departamento
{
    public int Id { get; set; }

    [Display(Name = "Cod. Dpto")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "El campo {0} debe tener exactamente {1} caracteres.")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe contener exactamente 2 dígitos.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string CodDepto { get; set; } = null!;

    [Display(Name = "Departamento")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Código INIDE")]
    [RegularExpression(@"^(\d{2})?$", ErrorMessage = "El código debe ser de 2 dígitos o estar vacío.")]
    public string? CodINIDE { get; set; }

    public ICollection<Municipio>? Municipios { get; set; }

    public int MunicipiosNumber => Municipios == null ? 0 : Municipios.Count;
}