using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.DTOs
{
    public class BulkResponseDTO
    {
        public bool Success { get; set; }
        public int Total { get; set; }
        public string? Message { get; set; }
    }
}