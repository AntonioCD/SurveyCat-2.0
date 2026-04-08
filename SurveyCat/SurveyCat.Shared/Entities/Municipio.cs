using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Municipio
{
    public int Id { get; set; }

    [Display(Name = "Cod. Municipio")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "El campo {0} debe tener exactamente {1} caracteres.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "El código debe contener exactamente 4 dígitos.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string CodMuni { get; set; } = null!;

    [Display(Name = "Municipio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Código INIDE")]
    [RegularExpression(@"^(\d{4})?$", ErrorMessage = "El código debe ser de 4 dígitos o estar vacío.")]
    public string? CodINIDE { get; set; }

    public int DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; } 

    public ICollection<BarrioComarca>? BarriosComarcas { get; set; }

    [Display(Name = "Barrios/Comarcas")]
    public int BarriosComarcasNumber => BarriosComarcas == null ? 0 : BarriosComarcas.Count;

    public ICollection<Sector>? Sectores { get; set; }

    [Display(Name = "Sectores")]
    public int SectoresNumber => Sectores == null ? 0 : Sectores.Count;

    //public ICollection<Persona>? Personas { get; set; }
}