using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Ficha
{
    public long Id { get; set; }

    // =========================================
    // UBICACIÓN
    // =========================================

    [Display(Name = "Municipio")]
    [Required]
    public int MunicipioId { get; set; }

    [Display(Name = "Sector")]
    [Required]
    public int SectorId { get; set; }

    [Display(Name = "Barrio / Comarca")]
    public int? BarrioComarcaId { get; set; }

    [Display(Name = "Cacerío")]
    public int? CacerioId { get; set; }

    [Display(Name = "Dirección")]
    [MaxLength(300)]
    public string? Direccion { get; set; }

    [MaxLength(10)]
    public string? Manzana { get; set; }

    [MaxLength(10)]
    public string? Lote { get; set; }

    // =========================================
    // IDENTIFICACIÓN
    // =========================================

    [MaxLength(20)]
    public string? CodParcela { get; set; }

    [Required]
    [MaxLength(20)]
    public string CodEncuesta { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? NombreFinca { get; set; }

    // =========================================
    // PERSONAL
    // =========================================

    [Required]
    public int EncuestadorId { get; set; }

    [Required]
    public int CoordinadorId { get; set; }

    [Required]
    public int TecnicoCatastralId { get; set; }

    // =========================================
    // ENCUESTA
    // =========================================

    [Required]
    public TipoEncuesta TipoEncuesta { get; set; }

    public TipoUsoParcela? TipoUso { get; set; }

    [MaxLength(300)]
    public string? DescripcionTipoUso { get; set; }

    public double? AreaEstimada { get; set; }

    public int? UnidadMedidaId { get; set; }

    public int? OrigenTierraId { get; set; }

    // =========================================
    // SERVIDUMBRES
    // =========================================

    public bool Servidumbre { get; set; }

    public int? ServidumbreAguaId { get; set; }

    public int? ServidumbrePaseId { get; set; }

    public int? ServidumbreOtraId { get; set; }

    // =========================================
    // CONFLICTOS
    // =========================================

    public bool PresentaConflicto { get; set; }

    // =========================================
    // CONTROL
    // =========================================

    [Required]
    public DateTime FechaEncuesta { get; set; } = DateTime.UtcNow;

    public bool VerificadoCoordinador { get; set; }

    [Required]
    public int EstadoId { get; set; }

    public string? Observacion { get; set; }

    // =========================================
    // INFORMANTE
    // =========================================

    [Required]
    public long InformanteId { get; set; }

    public int? RelacionInformanteParcelaId { get; set; }

    public int? RelacionInformantePropietarioId { get; set; }

    // =========================================
    // AUDITORÍA
    // =========================================

    public DateTime? CreatedDate { get; set; }

    [MaxLength(450)]
    public string? CreatorUserId { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MaxLength(450)]
    public string? UpdaterUserId { get; set; }

    // =========================================
    // NAVEGACIONES
    // =========================================

    public Municipio Municipio { get; set; } = null!;
    public Sector Sector { get; set; } = null!;
    public BarrioComarca? BarrioComarca { get; set; }
    public Caserio? Caserio { get; set; }

    public PersonalEncuesta Encuestador { get; set; } = null!;
    public PersonalEncuesta Coordinador { get; set; } = null!;
    public PersonalEncuesta TecnicoCatastral { get; set; } = null!;

    public Persona Informante { get; set; } = null!;

    // =========================================
    // COLECCIONES
    // =========================================

    //public ICollection<Propietario> Propietarios { get; set; } = new List<Propietario>();

    //public ICollection<Familia> Familias { get; set; } = new List<Familia>();

    //public ICollection<Colindante> Colindantes { get; set; } = new List<Colindante>();

    //public ICollection<DocumentoAnexo> DocumentosAnexos { get; set; } = new List<DocumentoAnexo>();

    //public ICollection<Conflicto> Conflictos { get; set; } = new List<Conflicto>();
}
