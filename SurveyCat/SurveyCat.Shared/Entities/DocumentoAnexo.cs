using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class DocumentoAnexo
{
    public long Id { get; set; }

    // =========================================
    // RELACIONES PRINCIPALES
    // =========================================

    [Display(Name = "Ficha")]
    [Required(ErrorMessage = "La ficha es obligatoria.")]
    public long FichaId { get; set; }

    public Ficha? Ficha { get; set; }

    [Display(Name = "Documento")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int DocumentoId { get; set; }

    public Diccionario? Documento { get; set; }

    // =========================================
    // INFORMACIÓN DEL DOCUMENTO
    // =========================================

    [Display(Name = "Descripción")]
    [MaxLength(300, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Número de Páginas")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero.")]
    public int NumeroPaginas { get; set; } = 1;

    [Display(Name = "Código")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string? Codigo { get; set; }

    // =========================================
    // NAVEGACIONES
    // =========================================

    public ICollection<Adjunto>? Adjuntos { get; set; }

    // =========================================
    // CONTADORES
    // =========================================

    public int AdjuntosNumber => Adjuntos == null ? 0 : Adjuntos.Count;
}