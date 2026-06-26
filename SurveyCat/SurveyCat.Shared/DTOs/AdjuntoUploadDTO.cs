using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class AdjuntoUploadDTO
    {
        public long DocumentoAnexoId { get; set; }
        public int ItemPagina { get; set; }
        public string NombreArchivo { get; set; } = null!;
        public string CodEncuesta { get; set; } = null!;
    }
}