using SurveyCat.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyCat.Shared.Entities
{
    public class EncuestaAutorizada
    {
        private string _codEncuesta = string.Empty;

        public long Id { get; set; }

        [Display(Name = "Código de Encuesta")]
        [Required(ErrorMessage = "El código de encuesta no ha sido generado.")]
        [MaxLength(25)]
        public string CodEncuesta
        {
            get => _codEncuesta;
            set => _codEncuesta = value?.Trim().ToUpper() ?? string.Empty;
        }

        [Display(Name = "Tipo de Sector")]
        [Range(1, 2, ErrorMessage = "El tipo de sector debe ser 1 (Urbano) o 2 (Rural).")]
        public TipoSector TipoSector { get; set; }

        [Display(Name = "Municipio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Municipio.")]
        [Required]
        public int MunicipioId { get; set; }

        public Municipio? Municipio { get; set; }

        [Display(Name = "Barrio / Comarca")]
        public int? BarrioComarcaId { get; set; }

        public BarrioComarca? BarrioComarca { get; set; }

        [Display(Name = "Caserío")]
        public int? CaserioId { get; set; }

        public Caserio? Caserio { get; set; }

        [Required]
        public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(450)]
        public string UsuarioCargaId { get; set; } = null!;

        [ForeignKey(nameof(UsuarioCargaId))]
        public User? User { get; set; }

        [Display(Name = "Observación")]
        public string? Observacion { get; set; }
    }
}