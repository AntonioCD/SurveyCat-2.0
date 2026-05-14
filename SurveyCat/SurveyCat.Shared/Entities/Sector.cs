using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities
{
    public class Sector
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de Sector")]
        [Range(1, 2, ErrorMessage = "El tipo de sector debe ser 1 (Urbano) o 2 (Rural).")]
        public TipoSector TipoSector { get; set; }

        [Display(Name = "Num. Sector")]
        [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string NumeroSector { get; set; } = null!;

        [Display(Name = "Cod. Sector-Dpto")]
        [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string SectorDepto { get; set; } = null!;

        [Display(Name = "Cod. Sector-Muni")]
        [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string SectorMuni { get; set; } = null!;

        [Display(Name = "Municipio")]
        public int MunicipioId { get; set; }

        public Municipio? Municipio { get; set; } = null!;

        public ICollection<Ficha>? Fichas { get; set; }
    }
}