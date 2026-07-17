using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Colindantes;

public partial class ColindanteEdit
{
    private Colindante? colindante;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int Id { get; set; }
    [Parameter] public int FichaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Colindante>($"api/colindantes/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=3");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            colindante = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        colindante!.Ficha = null;
        colindante.Persona = null;
        colindante.PuntoCardinal = null;
        colindante.Conflicto = null;
        colindante.ViaGestion = null;

        var responseHttp = await Repository.PutAsync<Colindante>("api/colindantes", colindante);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        // Volver a la ficha con la pestaña de Colindantes activa (tab=3)
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=3");
        Snackbar.Add("Colindante guardado exitosamente.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=3");
    }
}