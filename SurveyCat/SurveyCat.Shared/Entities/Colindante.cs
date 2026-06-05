using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Colindante
{
    public long Id { get; set; }

    // =========================================
    // RELACIONES PRINCIPALES
    // =========================================

    [Display(Name = "Ficha")]
    [Required(ErrorMessage = "La ficha es obligatoria.")]
    public long FichaId { get; set; }

    public Ficha? Ficha { get; set; }

    [Display(Name = "Punto Cardinal")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Punto Cardinal.")]
    public int PuntoCardinalId { get; set; }

    public Diccionario? PuntoCardinal { get; set; }

    [Display(Name = "Persona")]
    [Required(ErrorMessage = "La persona es obligatoria.")]
    public long PersonaId { get; set; }

    public Persona? Persona { get; set; }

    // =========================================
    // CONFLICTO
    // =========================================

    [Display(Name = "Presenta Conflicto")]
    public bool PresentaConflicto { get; set; } = false;

    [Display(Name = "Conflicto")]
    public int? ConflictoId { get; set; }

    public Diccionario? Conflicto { get; set; }

    [Display(Name = "Vía de Gestión")]
    public int? ViaGestionId { get; set; }

    public Diccionario? ViaGestion { get; set; }
}