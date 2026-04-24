using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.PersonalEncuestas;

public partial class PersonalEncuestaForm
{
    private EditContext editContext = null!;

    private List<Persona>? personas;
    private Persona? selectedPersona = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [EditorRequired, Parameter] public PersonalEncuesta PersonalEncuesta { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnInitializedAsync()
    {
        editContext = new(PersonalEncuesta);

        await LoadPersonasAsync();

        if (PersonalEncuesta.Id != 0)
        {
            selectedPersona = PersonalEncuesta.Persona;
        }
    }

    private async Task LoadPersonasAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Persona>>("/api/personas/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        personas = responseHttp.Response;
    }

    private void PersonaChangedAsync(Persona persona)
    {
        selectedPersona = persona;
        PersonalEncuesta.PersonaId = persona.Id;
    }

    private async Task<IEnumerable<Persona>> SearchPersona(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return personas!;
        }

        return personas!
            .Where(c => c.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
}