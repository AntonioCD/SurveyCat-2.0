using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities
{
    public class Departamento
    {
        public int Id { get; set; }

        [Display(Name = "Cod. Dpto")]
        [MaxLength(2, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodDepto { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(2, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string? CodINIDE { get; set; }
    }
}