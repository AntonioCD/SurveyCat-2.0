using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities
{
    public class Jornada
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de Jornada")]
        [MaxLength(200, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Código Jornada")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El campo {0} debe tener exactamente {1} caracteres.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "El código debe contener exactamente 3 dígitos.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodJornada { get; set; } = null!;

        [Display(Name = "Fecha de Inicio")]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Activo { get; set; } = false;

        public ICollection<Ficha>? Fichas { get; set; }
    }
}