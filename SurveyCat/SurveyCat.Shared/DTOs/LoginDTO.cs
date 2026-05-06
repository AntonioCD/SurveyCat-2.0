using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Username { get; set; } = null!;

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} carácteres.")]
    public string Password { get; set; } = null!;
}