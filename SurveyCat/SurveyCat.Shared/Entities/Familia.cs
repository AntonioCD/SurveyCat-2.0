using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Familia
{
    public long Id { get; set; }

    // =========================================
    // RELACIONES PRINCIPALES
    // =========================================

    [Display(Name = "Ficha")]
    [Required(ErrorMessage = "La ficha es obligatoria.")]
    public long FichaId { get; set; }

    public Ficha? Ficha { get; set; }

    [Display(Name = "Persona")]
    [Required(ErrorMessage = "La persona es obligatoria.")]
    public long PersonaId { get; set; }

    public Persona? Persona { get; set; }

    // =========================================
    // INFORMACIÓN FAMILIAR
    // =========================================

    [Display(Name = "Item")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero.")]
    public int? Item { get; set; }

    [Display(Name = "Parentesco")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int ParentescoId { get; set; }

    public Diccionario? Parentesco { get; set; }
}