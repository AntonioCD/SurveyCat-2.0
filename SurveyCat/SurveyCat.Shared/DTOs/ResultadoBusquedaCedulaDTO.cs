using SurveyCat.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class ResultadoBusquedaCedulaDTO
    {
        public bool EncontradoEnSistema { get; set; }
        public bool EncontradoEnPadron { get; set; }
        public Persona? PersonaExistente { get; set; }
        public PadronPersonaDTO? DatosPadron { get; set; }
    }
}