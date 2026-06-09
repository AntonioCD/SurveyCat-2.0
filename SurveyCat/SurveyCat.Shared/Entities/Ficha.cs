using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Municipio.")]
    [Required]
    public int MunicipioId { get; set; }

    public Municipio? Municipio { get; set; }

    [Display(Name = "Sector")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Sector.")]
    [Required]
    public int SectorId { get; set; }

    public Sector? Sector { get; set; }

    [Display(Name = "Barrio / Comarca")]
    public int? BarrioComarcaId { get; set; }

    public BarrioComarca? BarrioComarca { get; set; }

    [Display(Name = "Caserío")]
    public int? CaserioId { get; set; }

    public Caserio? Caserio { get; set; }

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

    [Display(Name = "Código de Parcela")]
    [MaxLength(20)]
    public string? CodParcela { get; set; }

    [Display(Name = "Código de Encuesta")]
    [Required(ErrorMessage = "El código de encuesta no ha sido generado.")]
    [MaxLength(20)]
    public string CodEncuesta { get; set; } = null!;

    [Display(Name = "Nombre de Finca")]
    [MaxLength(300)]
    public string? NombreFinca { get; set; }

    // =========================================
    // PERSONAL
    // =========================================

    [Display(Name = "Encuestador")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Encuestador.")]
    [Required]
    public int EncuestadorId { get; set; }

    [ForeignKey("EncuestadorId")]
    [InverseProperty("FichasEncuestador")]
    public PersonalEncuesta? Encuestador { get; set; }

    [Display(Name = "Coordinador")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Coordinador.")]
    [Required]
    public int CoordinadorId { get; set; }

    [ForeignKey("CoordinadorId")]
    [InverseProperty("FichasCoordinador")]
    public PersonalEncuesta? Coordinador { get; set; }

    [Display(Name = "Técnico Catastral")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Técnico Catastral.")]
    [Required]
    public int TecnicoCatastralId { get; set; }

    [ForeignKey("TecnicoCatastralId")]
    [InverseProperty("FichasTecnicoCatastral")]
    public PersonalEncuesta? TecnicoCatastral { get; set; }

    // =========================================
    // ENCUESTA
    // =========================================

    [Display(Name = "Tipo de Encuesta")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Tipo de Encuesta.")]
    [Required]
    public TipoEncuesta TipoEncuesta { get; set; }

    [Display(Name = "Tipo de Uso")]
    public TipoUsoParcela? TipoUso { get; set; }

    [Display(Name = "Descripción")]
    [MaxLength(300)]
    public string? DescripcionTipoUso { get; set; }

    [Display(Name = "Area Estimada")]
    public double? AreaEstimada { get; set; }

    [Display(Name = "Unidad de Medida")]
    public int? UnidadMedidaId { get; set; }

    [ForeignKey("UnidadMedidaId")]
    public Diccionario? UnidadMedida { get; set; }

    [Display(Name = "Origen de la Tierra")]
    public int? OrigenTierraId { get; set; }

    [ForeignKey("OrigenTierraId")]
    public Diccionario? OrigenTierra { get; set; }

    // =========================================
    // SERVIDUMBRES
    // =========================================

    [Display(Name = "¿Tiene Servidumbre?")]
    public bool Servidumbre { get; set; } = false;

    [Display(Name = "Tipo Servidumbre Agua")]
    public int? ServidumbreAguaId { get; set; }

    [ForeignKey("ServidumbreAguaId")]
    public Diccionario? ServidumbreAgua { get; set; }

    [Display(Name = "Tipo Servidumbre Pase")]
    public int? ServidumbrePaseId { get; set; }

    [ForeignKey("ServidumbrePaseId")]
    public Diccionario? ServidumbrePase { get; set; }

    [Display(Name = "Tipo Servidumbre Otra")]
    public int? ServidumbreOtraId { get; set; }

    [ForeignKey("ServidumbreOtraId")]
    public Diccionario? ServidumbreOtra { get; set; }

    // =========================================
    // CONFLICTOS
    // =========================================

    [Display(Name = "¿Presenta Conflicto?")]
    public bool PresentaConflicto { get; set; } = false;

    // =========================================
    // CONTROL
    // =========================================

    [Display(Name = "Fecha de Encuesta")]
    [Required]
    public DateTime FechaEncuesta { get; set; } = DateTime.UtcNow;

    [Display(Name = "¿Verificada por el Coordinador?")]
    public bool VerificadoCoordinador { get; set; } = false;

    [Display(Name = "Estado")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Estado.")]
    [Required]
    public int EstadoId { get; set; }

    [ForeignKey("EstadoId")]
    public Diccionario? Estado { get; set; }

    [Display(Name = "Observación")]
    public string? Observacion { get; set; }

    // =========================================
    // INFORMANTE
    // =========================================

    [Display(Name = "Informante")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Informante.")]
    [Required]
    public long InformanteId { get; set; }

    public Persona? Informante { get; set; }

    [Display(Name = "Relación Informante-Parcela")]
    public int? RelacionInformanteParcelaId { get; set; }

    [ForeignKey("RelacionInformanteParcelaId")]
    public Diccionario? RelacionInformanteParcela { get; set; }

    [Display(Name = "Relación Informante-Propietario")]
    public int? RelacionInformantePropietarioId { get; set; }

    [ForeignKey("RelacionInformantePropietarioId")]
    public Diccionario? RelacionInformantePropietario { get; set; }

    // =========================================
    // AUDITORÍA
    // =========================================

    [Display(Name = "Fecha Creación")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Creado Por")]
    [MaxLength(450)]
    public string? CreatorUserId { get; set; }

    [Display(Name = "Fecha Actualización")]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Actualizado Por")]
    [MaxLength(450)]
    public string? UpdaterUserId { get; set; }

    // =========================================
    // NOT MAPPED
    // =========================================
    [NotMapped]
    [Display(Name = "Consecutivo")]
    public string? Consecutivo { get; set; }

    // =========================================
    // COLECCIONES
    // =========================================

    public ICollection<Propietario>? Propietarios { get; set; }

    public ICollection<Familia>? Familias { get; set; }

    public ICollection<Colindante>? Colindantes { get; set; }

    public ICollection<Conflicto>? Conflictos { get; set; }

    //public ICollection<DocumentoAnexo> DocumentosAnexos { get; set; } = new List<DocumentoAnexo>();

    // =========================================
    // CONTADORES
    // =========================================

    public int PropietariosNumber => Propietarios == null ? 0 : Propietarios.Count;

    public int FamiliasNumber => Familias == null ? 0 : Familias.Count;

    public int ColindantesNumber => Colindantes == null ? 0 : Colindantes.Count;

    public int ConflictosNumber => Conflictos == null ? 0 : Conflictos.Count;
}