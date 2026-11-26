using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs;

public class FichaEstadoResponseDto
{
    public string CodEncuesta { get; set; } = string.Empty;
    public int EstadoId { get; set; }
    public string? EstadoNombre { get; set; }
}