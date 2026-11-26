using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs;

public class ConsultaEstadosRequestDto
{
    public List<string> CodEncuestas { get; set; } = new();
}