using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;
using static MudBlazor.Colors;

namespace SurveyCat.Frontend.Components.Pages.Personas;

public partial class PersonaForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaEstadoCivil = new();
    private List<Diccionario> listaProfesion = new();
    private List<Diccionario> listaTipoIdentificacion = new();
    private List<Diccionario> listaTipoPersonaJuridica = new();
    private List<Departamento>? departamentos;
    private List<Municipio>? municipios;
    private List<BarrioComarca>? barriosComarcas;
    private List<Caserio>? caserios;

    private Diccionario? selectedTipoIdentificacion = new();
    private Diccionario? selectedEstadoCivil = new();
    private Diccionario? selectedProfesion = new();
    private Diccionario? selectedTipoPersonaJuridica = new();
    private Departamento? selectedDepartamento = new();
    private Municipio? selectedMunicipio = new();
    private BarrioComarca? selectedBarrioComarca = new();
    private Caserio? selectedCaserio = new();

    public IMask maskIdentificacion = new RegexMask(@"^[a-zA-Z0-9]*$");

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [EditorRequired, Parameter] public Persona Persona { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnInitializedAsync()
    {
        editContext = new(Persona);

        // Cargamos lo básico que siempre debe estar (Catálogos y Deptos)
        await LoadDiccionariosAsync();
        await LoadDepartamentosAsync();

        if (Persona.Id != 0)
        {
            // 1. Cargas en cascada condicionales
            if (Persona.MunicipioId.HasValue)
            {
                await LoadMunicipiosAsync(Persona.MunicipioId.Value);
                selectedMunicipio = Persona.Municipio;

                // Cargamos el depto desde el municipio si existe
                selectedDepartamento = Persona.Municipio?.Departamento;
            }

            if (Persona.BarrioComarcaId.HasValue)
            {
                await LoadBarriosComarcasAsync(Persona.BarrioComarcaId.Value);
                selectedBarrioComarca = Persona.BarrioComarca;
            }

            if (Persona.CaserioId.HasValue)
            {
                await LoadCaseriosAsync(Persona.CaserioId.Value);
                selectedCaserio = Persona.Caserio;
            }

            // 2. Asignación segura de objetos de diccionario
            // Usamos el operador ?. para que si es null, la variable selected también sea null
            selectedTipoIdentificacion = Persona.TipoIdentificacion;
            selectedEstadoCivil = Persona.EstadoCivil;
            selectedProfesion = Persona.Profesion;
            selectedTipoPersonaJuridica = Persona.TipoPersonaJuridica;
        }
    }

    private async Task LoadDiccionariosAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Diccionario>>("/api/diccionarios/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        diccionarios = responseHttp.Response;

        if (diccionarios != null)
        {
            listaEstadoCivil = diccionarios.Where(x => x.Catalogo == Catalogos.EstadoCivil).ToList();
            listaProfesion = diccionarios.Where(x => x.Catalogo == Catalogos.Profesion).ToList();
            listaTipoIdentificacion = diccionarios.Where(x => x.Catalogo == Catalogos.TipoIdentificacion).ToList();
            listaTipoPersonaJuridica = diccionarios.Where(x => x.Catalogo == Catalogos.TipoPersonaJuridica).ToList();
        }
    }

    private async Task LoadDepartamentosAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Departamento>>("/api/departamentos/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        departamentos = responseHttp.Response;
    }

    private async Task LoadMunicipiosAsync(int departamentoId)
    {
        var responseHttp = await Repository.GetAsync<List<Municipio>>($"/api/municipios/combo/{departamentoId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        municipios = responseHttp.Response;
    }

    private async Task LoadBarriosComarcasAsync(int municipioId)
    {
        var responseHttp = await Repository.GetAsync<List<BarrioComarca>>($"/api/barriosComarcas/combo/{municipioId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        barriosComarcas = responseHttp.Response;
    }

    private async Task LoadCaseriosAsync(int comarcaId)
    {
        var responseHttp = await Repository.GetAsync<List<Caserio>>($"/api/caserios/combo/{comarcaId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        caserios = responseHttp.Response;
    }

    private void TipoIdentificacionChanged(Diccionario tipoIdentificacion)
    {
        selectedTipoIdentificacion = tipoIdentificacion;
        Persona.TipoIdentificacionId = tipoIdentificacion.Id;
    }

    private void EstadoCivilChanged(Diccionario estadoCivil)
    {
        selectedEstadoCivil = estadoCivil;
        Persona.EstadoCivilId = estadoCivil.Id;
    }

    private void ProfesionChanged(Diccionario profesion)
    {
        selectedProfesion = profesion;
        Persona.ProfesionId = profesion.Id;
    }

    private void TipoPersonaJuridicaChanged(Diccionario tipoPersonaJuridica)
    {
        selectedTipoPersonaJuridica = tipoPersonaJuridica;
        Persona.TipoPersonaJuridicaId = tipoPersonaJuridica.Id;
    }

    private async Task DepartamentoChangedAsync(Departamento departamento)
    {
        selectedDepartamento = departamento;
        selectedMunicipio = new Municipio();
        selectedBarrioComarca = new BarrioComarca();
        selectedCaserio = new Caserio();
        municipios = null;
        barriosComarcas = null;
        caserios = null;
        await LoadMunicipiosAsync(departamento.Id);
    }

    private async Task MunicipioChangedAsync(Municipio municipio)
    {
        selectedMunicipio = municipio;
        Persona.MunicipioId = municipio.Id;
        selectedBarrioComarca = new BarrioComarca();
        selectedCaserio = new Caserio();
        barriosComarcas = null;
        caserios = null!;
        await LoadBarriosComarcasAsync(municipio.Id);
    }

    private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    {
        selectedBarrioComarca = barrioComarca;
        Persona.BarrioComarcaId = barrioComarca.Id;
        selectedCaserio = new Caserio();
        caserios = null!;
        await LoadCaseriosAsync(barrioComarca.Id);
    }

    private void CaserioChanged(Caserio caserio)
    {
        selectedCaserio = caserio;
        Persona.CaserioId = caserio.Id;
    }

    private async Task<IEnumerable<Diccionario>> SearchTipoIdentificacion(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaTipoIdentificacion!;
        }

        return listaTipoIdentificacion!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchEstadoCivil(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaEstadoCivil!;
        }

        return listaEstadoCivil!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchProfesion(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaProfesion!;
        }

        return listaProfesion!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchTipoPersonaJuridica(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaTipoPersonaJuridica!;
        }

        return listaTipoPersonaJuridica!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Departamento>> SearchDepartamento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return departamentos!;
        }

        return departamentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Municipio>> SearchMunicipio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return municipios!;
        }

        return municipios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<BarrioComarca>> SearchBarrioComarca(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return barriosComarcas!;
        }

        return barriosComarcas!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Caserio>> SearchCaserio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return caserios!;
        }

        return caserios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
}