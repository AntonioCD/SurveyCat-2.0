using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Personas;

public partial class PersonaEdit
{
    private Persona? persona;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public long Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Persona>($"api/personas/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("personas");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            persona = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        persona!.TipoIdentificacion = null;
        persona.EstadoCivil = null;
        persona.Profesion = null;
        persona.Municipio = null;
        persona.BarrioComarca = null;
        persona.Caserio = null;
        persona.TipoPersonaJuridica = null!;

        var responseHttp = await Repository.PutAsync("api/personas", persona);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        //Return();
        Snackbar.Add("Registro guardado", Severity.Success);

        MudDialog.Close(DialogResult.Ok(true));
    }

    //private void Return()
    //{
    //    //NavigationManager.NavigateTo($"/personas");
    //}

    private void Return()
    {
        MudDialog.Cancel();
    }
}