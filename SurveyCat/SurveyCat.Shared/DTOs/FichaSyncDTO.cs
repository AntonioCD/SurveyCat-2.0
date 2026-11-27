using SurveyCat.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SurveyCat.Shared.DTOs;

public class FichaSyncDTO
{
    // Si viene > 0 y existe en BD, se actualiza; si es 0, se busca por CodEncuesta o se crea.
    public long Id { get; set; }

    [Required(ErrorMessage = "El código de encuesta es obligatorio.")]
    [MaxLength(25)]
    public string CodEncuesta { get; set; } = null!;

    // Ubicación
    public int JornadaId { get; set; }

    public TipoSector TipoSector { get; set; }
    public int MunicipioId { get; set; }
    public int? SectorId { get; set; }
    public int? BarrioComarcaId { get; set; }
    public int? CaserioId { get; set; }

    [MaxLength(300)]
    public string? Direccion { get; set; }

    [MaxLength(10)]
    public string? Manzana { get; set; }

    [MaxLength(10)]
    public string? Lote { get; set; }

    [MaxLength(2)]
    public string? Piso { get; set; }

    [MaxLength(2)]
    public string? Apartamento { get; set; }

    // Identificación
    [MaxLength(25)]
    public string? CodParcela { get; set; }

    [MaxLength(300)]
    public string? NombreFinca { get; set; }

    // Personal
    public int EncuestadorId { get; set; }

    public int CoordinadorId { get; set; }
    public int TecnicoCatastralId { get; set; }

    // Encuesta
    public TipoEncuesta TipoEncuesta { get; set; }

    [MaxLength(25)]
    public string? CodSiscat { get; set; }

    public TipoUsoParcela? TipoUso { get; set; }

    [MaxLength(300)]
    public string? DescripcionTipoUso { get; set; }

    public double? AreaEstimada { get; set; }
    public int? UnidadMedidaId { get; set; }
    public int? OrigenTierraId { get; set; }

    // Servidumbres
    public bool Servidumbre { get; set; }

    public int? ServidumbreAguaId { get; set; }
    public int? ServidumbrePaseId { get; set; }
    public int? ServidumbreOtraId { get; set; }

    // Conflictos y Control
    public bool PresentaConflicto { get; set; }

    [Required]
    public DateTime FechaEncuesta { get; set; }

    public bool VerificadoCoordinador { get; set; }
    public int EstadoId { get; set; }
    public string? Observacion { get; set; }

    // Informante (Persona embebida para resolver o crear en SQL Server)
    public PersonaSyncDTO? Informante { get; set; }

    public int? RelacionInformanteParcelaId { get; set; }
    public int? RelacionInformantePropietarioId { get; set; }

    // Colecciones hijas
    public List<PropietarioSyncDTO> Propietarios { get; set; } = new();

    public List<OcupanteSyncDTO> Ocupantes { get; set; } = new();
    public List<ConflictoSyncDTO> Conflictos { get; set; } = new();
    public List<DocumentoAnexoSyncDTO> DocumentosAnexos { get; set; } = new();
}

public class PersonaSyncDTO
{
    public long Id { get; set; }
    public TipoPersona TipoPersona { get; set; }
    public int? TipoIdentificacionId { get; set; }

    [MaxLength(20)]
    public string? Identificacion { get; set; }

    [MaxLength(50)]
    public string? PrimerNombre { get; set; }

    [MaxLength(50)]
    public string? SegundoNombre { get; set; }

    [MaxLength(50)]
    public string? PrimerApellido { get; set; }

    [MaxLength(50)]
    public string? SegundoApellido { get; set; }

    [Required]
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = null!;

    public TipoGenero? Genero { get; set; }
    public int? Edad { get; set; }
    public int? EstadoCivilId { get; set; }
    public int? ProfesionId { get; set; }

    public int? MunicipioId { get; set; }
    public int? BarrioComarcaId { get; set; }
    public int? CaserioId { get; set; }

    [MaxLength(300)]
    public string? Direccion { get; set; }

    public int? TipoPersonaJuridicaId { get; set; }

    [MaxLength(100)]
    public string? RegistradaEn { get; set; }

    public DateTime? FechaRegistro { get; set; }
}

public class PropietarioSyncDTO
{
    [Required]
    public PersonaSyncDTO Persona { get; set; } = null!;

    public PerfilPropietario? Perfil { get; set; }

    [MaxLength(100)]
    public string? EspecificarPerfil { get; set; }

    public int? Hijos { get; set; }
    public int? Hijas { get; set; }

    public TipoDerecho TipoDerecho { get; set; }
    public bool PresentaDocumento { get; set; }
    public int? DocumentoId { get; set; }

    [MaxLength(200)]
    public string? AutorDocumento { get; set; }

    public DateTime? FechaDocumento { get; set; }

    public double? AreaTitulada { get; set; }
    public int? UnidadMedidaId { get; set; }
    public DateTime? FechaAdquisicion { get; set; }
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

public class OcupanteSyncDTO
{
    [Required]
    public PersonaSyncDTO Persona { get; set; } = null!;

    public int TipoOcupanteId { get; set; }
    public int? ParentescoId { get; set; }
    public int? Item { get; set; }
}

public class ConflictoSyncDTO
{
    public int TipoConflictoId { get; set; }
    public int ViaGestionId { get; set; }
    public bool ConEstado { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }
}

public class DocumentoAnexoSyncDTO
{
    public int DocumentoId { get; set; }

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    public int NumeroPaginas { get; set; } = 1;

    [MaxLength(50)]
    public string? Codigo { get; set; }

    public List<AdjuntoSyncDTO> Adjuntos { get; set; } = new();
}

public class AdjuntoSyncDTO
{
    public int ItemPagina { get; set; }

    [Required]
    [MaxLength(50)]
    public string NombreArchivo { get; set; } = null!;

    /// <summary>
    /// Contenido binario de la imagen/PDF codificado en Base64 (opcional si ya existe en servidor).
    /// </summary>
    public string? ContenidoBase64 { get; set; }
}

public class FichaSyncResultDTO
{
    public long FichaId { get; set; }
    public string CodEncuesta { get; set; } = null!;
    public bool FueActualizada { get; set; }
    public int TotalPropietarios { get; set; }
    public int TotalOcupantes { get; set; }
    public int TotalConflictos { get; set; }
    public int TotalDocumentosAnexos { get; set; }
    public int TotalAdjuntos { get; set; }
}