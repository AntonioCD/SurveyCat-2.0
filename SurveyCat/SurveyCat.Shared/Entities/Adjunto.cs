using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Adjunto
{
    public int Id { get; set; }

    // =========================================
    // RELACIONES PRINCIPALES
    // =========================================

    [Display(Name = "Documento Anexo")]
    [Required(ErrorMessage = "El documento anexo es obligatorio.")]
    public long DocumentoAnexoId { get; set; }

    public DocumentoAnexo? DocumentoAnexo { get; set; }

    // =========================================
    // INFORMACIÓN DEL ARCHIVO
    // =========================================

    [Display(Name = "Página")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor a cero.")]
    public int ItemPagina { get; set; }

    [Display(Name = "Nombre del Archivo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string NombreArchivo { get; set; } = null!;

    [Display(Name = "Ruta")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string Ruta { get; set; } = null!;
}