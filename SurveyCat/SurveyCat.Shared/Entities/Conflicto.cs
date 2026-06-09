using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Conflicto
{
    public long Id { get; set; }

    // =========================================
    // RELACIONES PRINCIPALES
    // =========================================

    [Display(Name = "Ficha")]
    [Required(ErrorMessage = "La ficha es obligatoria.")]
    public long FichaId { get; set; }

    public Ficha? Ficha { get; set; }

    // =========================================
    // INFORMACIÓN DEL CONFLICTO
    // =========================================

    [Display(Name = "Clase de Conflicto")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int ConflictoId { get; set; }

    public Diccionario? TipoConflicto { get; set; }

    [Display(Name = "Vía de Gestión")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int ViaGestionId { get; set; }

    public Diccionario? ViaGestion { get; set; }

    [Display(Name = "Conflicto con el Estado")]
    public bool ConEstado { get; set; } = false;

    [Display(Name = "Descripción")]
    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string? Descripcion { get; set; }
}