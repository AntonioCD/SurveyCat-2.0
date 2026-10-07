using SurveyCat.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class ResultadoBusquedaNombreDTO
    {
        public List<Persona> PersonasEnSistema { get; set; } = new();
        public List<PadronPersonaDTO> PersonasEnPadron { get; set; } = new();
    }
}