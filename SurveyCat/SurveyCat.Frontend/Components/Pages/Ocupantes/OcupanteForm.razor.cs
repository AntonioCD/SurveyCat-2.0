using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Ocupantes;

public partial class OcupanteForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaTipoOcupante = new();
    private List<Diccionario> listaParentesco = new();
    private Persona? persona = new();

    private Diccionario? selectedTipoOcupante;
    private Diccionario? selectedParentesco;

    // NUEVO: Constante y variable para el ID de "Familiar"
    private const string TIPO_FAMILIAR_NOMBRE = "Familiar";
    private long? _tipoFamiliarId;

    // NUEVO: Propiedades de validación condicional
    private bool ParentescoEsRequerido =>
        selectedTipoOcupante?.Id == _tipoFamiliarId;

    private bool ParentescoHabilitado =>
        selectedTipoOcupante != null && ParentescoEsRequerido;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public Ocupante Ocupante { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Ocupante == null)
        {
            Ocupante = new Ocupante();
        }

        if (editContext == null || editContext.Model != Ocupante)
        {
            editContext = new EditContext(Ocupante);
        }

        await LoadDiccionariosAsync();

        if (Ocupante.Id != 0)
        {
            selectedParentesco = listaParentesco.FirstOrDefault(x => x.Id == Ocupante.ParentescoId);
            selectedTipoOcupante = listaTipoOcupante.FirstOrDefault(x => x.Id == Ocupante.TipoOcupanteId);

            if (persona == null || persona.Id != Ocupante.PersonaId)
            {
                persona = await GetPersonaDetails(Ocupante.PersonaId);
            }
        }
        else
        {
            // Limpiar para nuevos registros
            selectedTipoOcupante = null;
            selectedParentesco = null;
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
            listaParentesco = diccionarios.Where(x => x.Catalogo == Catalogos.Parentesco).ToList();
            listaTipoOcupante = diccionarios.Where(x => x.Catalogo == Catalogos.TipoOcupante).ToList();

            // NUEVO: Obtener el ID del tipo "Familiar"
            var familiar = listaTipoOcupante.FirstOrDefault(x => x.Nombre == TIPO_FAMILIAR_NOMBRE);
            if (familiar != null)
            {
                _tipoFamiliarId = familiar.Id;
            }
        }
    }

    // MODIFICADO: Limpiar Parentesco si no es Familiar
    private void TipoOcupanteChanged(Diccionario? tipoOcupante)
    {
        selectedTipoOcupante = tipoOcupante;
        Ocupante.TipoOcupanteId = tipoOcupante?.Id ?? 0;

        // Si NO es Familiar, limpiar Parentesco
        if (!ParentescoEsRequerido)
        {
            selectedParentesco = null;
            Ocupante.ParentescoId = null;
        }

        editContext?.Validate();
        StateHasChanged();
    }

    private void ParentescoChanged(Diccionario? parentesco)
    {
        selectedParentesco = parentesco;
        Ocupante.ParentescoId = parentesco?.Id;

        editContext?.Validate();
    }

    private async Task<IEnumerable<Diccionario>> SearchTipoOcupante(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaTipoOcupante!;
        }

        return listaTipoOcupante!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    // MODIFICADO: Solo muestra parentescos si está habilitado
    private async Task<IEnumerable<Diccionario>> SearchParentesco(string searchText, CancellationToken token)
    {
        await Task.Delay(5);

        if (!ParentescoHabilitado)
        {
            return new List<Diccionario>();
        }

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaParentesco!;
        }

        return listaParentesco!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task ShowModalPersonaSearchAsync()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };

        var parameters = new DialogParameters<PersonaSearch>
        {
            { x => x.SoloNaturales, true }
        };

        var dialog = await DialogService.ShowAsync<PersonaSearch>("Buscar Persona", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is Persona personaSeleccionada)
        {
            var personaResult = await GetPersonaDetails(personaSeleccionada.Id);

            if (personaResult != null)
            {
                persona = personaResult;
                Ocupante.PersonaId = persona.Id;
                Snackbar.Add("Datos de la persona cargados con éxito.", Severity.Success);
            }
            else
            {
                Snackbar.Add("No se pudieron cargar los datos de la persona.", Severity.Warning);
            }

            StateHasChanged();
        }
    }

    private async Task<Persona?> GetPersonaDetails(long personaId)
    {
        var responseHttp = await Repository.GetAsync<Persona>($"api/personas/{personaId}");

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return null;
        }

        if (responseHttp.Response == null)
        {
            Snackbar.Add("No se encontraron los detalles de la persona.", Severity.Warning);
            return null;
        }

        return responseHttp.Response;
    }
}