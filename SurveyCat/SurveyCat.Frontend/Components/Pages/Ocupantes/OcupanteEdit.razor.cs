using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Ocupantes;

public partial class OcupanteEdit
{
    private Ocupante? ocupante;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int Id { get; set; }
    [Parameter] public int FichaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Ocupante>($"api/ocupantes/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            ocupante = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        ocupante!.Ficha = null;
        ocupante.Persona = null;
        ocupante.Parentesco = null;

        var responseHttp = await Repository.PutAsync<Ocupante>("api/ocupantes", ocupante);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        // Volver a la ficha con la pestaña de Núcleo Ocupante activa (tab=2)
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
        Snackbar.Add("Ocupante guardado exitosamente.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}