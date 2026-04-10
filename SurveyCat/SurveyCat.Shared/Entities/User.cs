using Microsoft.AspNetCore.Identity;
using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class User : IdentityUser
{
    [Display(Name = "Tipo de Usuario")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un {0}.")]
    public UserType UserType { get; set; }

    public bool Activo { get; set; } = true;
}