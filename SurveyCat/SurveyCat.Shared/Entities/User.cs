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
    public PersonalEncuesta? PersonalEncuesta { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Ficha>? Fichas { get; set; }
}