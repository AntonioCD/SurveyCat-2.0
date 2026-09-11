using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class PadronPersonaDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string PrimerNombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string? Domicilio { get; set; }

        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}".Replace("  ", " ").Trim();
    }
}