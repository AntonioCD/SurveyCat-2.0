using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.PersonalEncuestas;

public partial class PersonalEncuestaForm
{
    private EditContext editContext = null!;

    //private List<Persona>? personas;
    //private Persona? selectedPersona = new();
    private Persona? persona = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public PersonalEncuesta PersonalEncuesta { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        //editContext = new(PersonalEncuesta);

        //await LoadPersonasAsync();

        if (PersonalEncuesta == null)
        {
            PersonalEncuesta = new PersonalEncuesta();
        }

        // Recrear el EditContext si el modelo cambió
        if (editContext == null || editContext.Model != PersonalEncuesta)
        {
            editContext = new EditContext(PersonalEncuesta);
        }

        if (PersonalEncuesta.Id != 0)
        {
            //selectedPersona = PersonalEncuesta.Persona;

            if (persona == null || persona.Id != PersonalEncuesta.PersonaId)
            {
                persona = await GetPersonaDetails(PersonalEncuesta.PersonaId);
            }
        }
    }

    //private async Task LoadPersonasAsync()
    //{
    //    var responseHttp = await Repository.GetAsync<List<Persona>>("/api/personas/combo");

    //    if (responseHttp.Error)
    //    {
    //        var message = await responseHttp.GetErrorMessageAsync();
    //        Snackbar.Add(message!, Severity.Error);
    //        return;
    //    }

    //    personas = responseHttp.Response;
    //}

    //private void PersonaChangedAsync(Persona persona)
    //{
    //    selectedPersona = persona;
    //    PersonalEncuesta.PersonaId = persona.Id;
    //}

    //private async Task<IEnumerable<Persona>> SearchPersona(string searchText, CancellationToken token)
    //{
    //    await Task.Delay(5);
    //    if (string.IsNullOrWhiteSpace(searchText))
    //    {
    //        return personas!;
    //    }

    //    return personas!
    //        .Where(c => c.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
    //        .ToList();
    //}

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
                PersonalEncuesta.PersonaId = persona.Id;

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