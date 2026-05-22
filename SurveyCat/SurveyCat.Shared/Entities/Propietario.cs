using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Propietario
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
    // PERFIL DEL PROPIETARIO
    // =========================================

    [Display(Name = "Perfil")]
    public PerfilPropietario? Perfil { get; set; }

    [Display(Name = "Especificar Perfil")]
    [MaxLength(100)]
    public string? EspecificarPerfil { get; set; }

    // =========================================
    // INFORMACIÓN FAMILIAR
    // =========================================

    [Display(Name = "No. Hijos")]
    [Range(0, int.MaxValue)]
    public int? Hijos { get; set; }

    [Display(Name = "No. Hijas")]
    [Range(0, int.MaxValue)]
    public int? Hijas { get; set; }

    // =========================================
    // DERECHO SOBRE LA PARCELA
    // =========================================

    [Display(Name = "Tipo de Derecho")]
    [Required(ErrorMessage = "El tipo de derecho es obligatorio.")]
    public TipoDerecho TipoDerecho { get; set; }

    // =========================================
    // DOCUMENTACIÓN
    // =========================================

    [Display(Name = "Presenta Documento")]
    public bool PresentaDocumento { get; set; } = false;

    [Display(Name = "Documento")]
    public int? DocumentoId { get; set; }

    public Diccionario? Documento { get; set; }

    [Display(Name = "Autor Documento")]
    [MaxLength(200)]
    public string? AutorDocumento { get; set; }

    [Display(Name = "Fecha Documento")]
    public DateTime? FechaDocumento { get; set; }

    // =========================================
    // INFORMACIÓN REGISTRAL
    // =========================================

    [Display(Name = "Área Titulada")]
    public double? AreaTitulada { get; set; }

    [Display(Name = "Unidad de Medida")]
    public int? UnidadMedidaId { get; set; }

    public Diccionario? UnidadMedida { get; set; }

    [Display(Name = "Fecha Adquisición")]
    public DateTime? FechaAdquisicion { get; set; }

    [Display(Name = "Fecha Registro")]
    public DateTime? FechaRegistro { get; set; }

    [MaxLength(20)]
    public string? Finca { get; set; }

    [MaxLength(50)]
    public string? Tomo { get; set; }

    [MaxLength(50)]
    public string? Folio { get; set; }

    [MaxLength(20)]
    public string? Asiento { get; set; }
}