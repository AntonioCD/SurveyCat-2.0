using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Familias;

public partial class FamiliaForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaParentesco = new();
    private Persona? persona = new();

    private Diccionario? selectedParentesco = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public Familia Familia { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Familia == null)
        {
            Familia = new Familia();
        }

        // Recrear el EditContext si el modelo cambió
        if (editContext == null || editContext.Model != Familia)
        {
            editContext = new EditContext(Familia);
        }

        await LoadDiccionariosAsync();

        if (Familia.Id != 0)
        {
            selectedParentesco = listaParentesco.Where(x => x.Id == Familia.ParentescoId).FirstOrDefault();

            if (persona == null || persona.Id != Familia.PersonaId)
            {
                persona = await GetPersonaDetails(Familia.PersonaId);
            }
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
        }
    }

    private void ParentescoChanged(Diccionario parentesco)
    {
        selectedParentesco = parentesco;
        Familia.ParentescoId = parentesco.Id;
    }

    private async Task<IEnumerable<Diccionario>> SearchParentesco(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
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
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<PersonaSearch>("Buscar Persona", options);
        var result = await dialog.Result;

        // Verificamos si el usuario seleccionó un registro en el modal
        if (!result.Canceled && result.Data is Persona personaSeleccionada)
        {
            // 1. LLAMADO ASÍNCRONO: Esperamos a que la API traiga los detalles completos
            var personaResult = await GetPersonaDetails(personaSeleccionada.Id);

            // 2. VALIDACIÓN: Evaluamos si devolvió a la persona o si falló (null)
            if (personaResult != null)
            {
                // Asignamos el resultado a la variable que maneja tu formulario
                persona = personaResult;
                Familia.PersonaId = persona.Id;

                // Si necesitas refrescar cascadas asociadas al informante (municipios, depto, etc.), este es el lugar:
                // await CargarCascadasDelInformanteAsync(informante);

                Snackbar.Add("Datos de la persona cargados con éxito.", Severity.Success);
            }
            else
            {
                // Opcional: Lógica en caso de que no se haya podido recuperar la data completa
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
            return null; // Retorna null en caso de error
        }

        if (responseHttp.Response == null)
        {
            Snackbar.Add("No se encontraron los detalles de la persona.", Severity.Warning);
            return null; // Retorna null si la API respondió vacío
        }

        return responseHttp.Response; // Retorna la Persona encontrada con éxito
    }
}