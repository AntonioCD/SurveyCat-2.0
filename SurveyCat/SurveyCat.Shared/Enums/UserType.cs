using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Enums
{
    public enum UserType
    {
        [Description("Administrador")]
        Admin = 1,

        // --- Roles de Operación de Campo ---
        [Description("Coordinador de Campo")]
        CoordinadorCampo = 2,

        [Description("Jefe de Grupo")]
        JefeGrupo = 3,

        [Description("Encuestador")]
        Encuestador = 4,

        // --- Roles de Control de Calidad (QA) ---
        [Description("Coordinador de Control de Calidad")]
        CoordinadorControlCalidad = 5,

        [Description("Técnico de Control de Calidad")]
        TecnicoControlCalidad = 6
    }
}