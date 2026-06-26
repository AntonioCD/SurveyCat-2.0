using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class DashboardResponseDTO
    {
        // 1. KPIs (Tarjetas superiores)
        public int TotalFichas { get; set; }
        public int TotalFichasControlCalidad { get; set; }
        public int TotalFichasEnCorrecion { get; set; }
        public int TotalFichasAprobadas { get; set; }

        // 2. Gráfico (Fichas por mes)
        public List<string> MesesGrafico { get; set; } = [];

        public List<double> ValoresGrafico { get; set; } = [];

        // 3. Tabla inferior (Actividad reciente)
        public List<FichaRecienteDTO> FichasRecientes { get; set; } = [];
    }
}