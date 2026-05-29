using SurveyCat.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyCat.Shared.Entities;

public class Persona
{
    private string? _identificacion;
    private string? _primerNombre;
    private string? _segundoNombre;
    private string? _primerApellido;
    private string? _segundoApellido;
    private string _nombreCompleto = null!;

    public long Id { get; set; }

    [Display(Name = "Tipo de Persona")]
    [Range(1, 2, ErrorMessage = "Seleccione el Tipo de Persona.")]
    public TipoPersona TipoPersona { get; set; }

    [Display(Name = "Tipo de Identificación")]
    public int? TipoIdentificacionId { get; set; }

    public Diccionario? TipoIdentificacion { get; set; }

    [Display(Name = "No. Identificación")]
    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? Identificacion
    {
        get => _identificacion;
        set => _identificacion = value?.ToUpper();
    }

    [Display(Name = "Primer Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? PrimerNombre
    {
        get => _primerNombre;
        set { _primerNombre = value; ActualizarNombreCompleto(); }
    }

    [Display(Name = "Segundo Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? SegundoNombre
    {
        get => _segundoNombre;
        set { _segundoNombre = value; ActualizarNombreCompleto(); }
    }

    [Display(Name = "Primer Apellido")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? PrimerApellido
    {
        get => _primerApellido;
        set { _primerApellido = value; ActualizarNombreCompleto(); }
    }

    [Display(Name = "Segundo Apellido")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? SegundoApellido
    {
        get => _segundoApellido;
        set { _segundoApellido = value; ActualizarNombreCompleto(); }
    }

    [Display(Name = "Nombre Completo/Razón Social")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string NombreCompleto
    {
        get => _nombreCompleto;
        set => _nombreCompleto = LimpiarEspacios(value);
    }

    [Display(Name = "Género")]
    public TipoGenero? Genero { get; set; }

    [Display(Name = "Edad")]
    public int? Edad { get; set; }

    [Display(Name = "Estado Civil")]
    public int? EstadoCivilId { get; set; }

    public Diccionario? EstadoCivil { get; set; }

    [Display(Name = "Profesión")]
    public int? ProfesionId { get; set; }

    public Diccionario? Profesion { get; set; }

    [Display(Name = "Municipio")]
    public int? MunicipioId { get; set; }

    public Municipio? Municipio { get; set; }

    [Display(Name = "Barrio/Comarca")]
    public int? BarrioComarcaId { get; set; }

    public BarrioComarca? BarrioComarca { get; set; }

    [Display(Name = "Caserio")]
    public int? CaserioId { get; set; }

    public Caserio? Caserio { get; set; }

    [Display(Name = "Dirección")]
    [MaxLength(300, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? Direccion { get; set; }

    [Display(Name = "Tipo de Persona Jurídica")]
    public int? TipoPersonaJuridicaId { get; set; }

    public Diccionario? TipoPersonaJuridica { get; set; }

    [Display(Name = "Lugar de Registro")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
    public string? RegistradaEn { get; set; }

    [Display(Name = "Fecha de Registro")]
    public DateTime? FechaRegistro { get; set; }

    [Display(Name = "Fecha de Creación")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Creado Por")]
    [MaxLength(450, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? CreatorUserId { get; set; }

    [Display(Name = "Fecha de Actualización")]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Actualizado Por")]
    [MaxLength(450, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
    public string? UpdaterUserId { get; set; }

    public ICollection<Ficha>? Fichas { get; set; }

    // --- LÓGICA DE APOYO ---

    private void ActualizarNombreCompleto()
    {
        var partes = new[] { _primerNombre, _segundoNombre, _primerApellido, _segundoApellido };
        var concatenado = string.Join(" ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));

        // Solo actualizamos si el resultado no es vacío para no borrar
        // lo que el usuario pudo ingresar manualmente en NombreCompleto
        if (!string.IsNullOrWhiteSpace(concatenado))
        {
            _nombreCompleto = concatenado;
        }
    }

    private string LimpiarEspacios(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        // Reemplaza múltiples espacios por uno solo y recorta los extremos
        return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ").Trim();
    }
}