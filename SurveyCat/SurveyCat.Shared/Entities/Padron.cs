using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Padron
{
    public string Id { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Primer_Nombre { get; set; } = string.Empty;
    public string? Segundo_Nombre { get; set; }
    public string Primer_Apellido { get; set; } = string.Empty;
    public string? Segundo_Apellido { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? Domicilio { get; set; }
}