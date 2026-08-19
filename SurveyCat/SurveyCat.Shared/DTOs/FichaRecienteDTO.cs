using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs;

public class FichaRecienteDTO
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Departamento { get; set; } = null!;
    public DateTime FechaEncuesta { get; set; }
    public string Estado { get; set; } = null!;
}