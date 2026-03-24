using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class BarrioComarca
{
    public int Id { get; set; }

    [Display(Name = "Código")]
    [MaxLength(4, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string CodBarrioComarca { get; set; } = null!;

    [Display(Name = "Nombre")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "¿Es Barrio?")]
    public bool EsBarrio { get; set; } = true;

    public int MunicipioId { get; set; }
    public Municipio Municipio { get; set; } = null!;

    public ICollection<Caserio>? Caserios { get; set; }

    [Display(Name = "Caserios")]
    public int CaseriosNumber => Caserios == null ? 0 : Caserios.Count;
}