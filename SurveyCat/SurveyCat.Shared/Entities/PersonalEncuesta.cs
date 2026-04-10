using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities
{
    public class PersonalEncuesta
    {
        public int Id { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long PersonaId { get; set; }

        public Persona? Persona { get; set; }

        [Display(Name = "Usuario")]
        [MaxLength(450, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string? UserId { get; set; }

        [Display(Name = "Código")]
        [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Codigo { get; set; } = null!;

        [Display(Name = "Brigada")]
        [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Brigada { get; set; } = null!;

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public TipoRol TipoRol { get; set; }
    }
}