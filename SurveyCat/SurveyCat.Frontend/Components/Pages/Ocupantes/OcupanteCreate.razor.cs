using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Ocupantes;

public partial class OcupanteCreate
{
    private Ocupante ocupante = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }

    private async Task CreateAsync()
    {
        ocupante.FichaId = FichaId;
        var responseHttp = await Repository.PostAsync<Ocupante>("/api/ocupantes", ocupante);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        // Volver a la ficha con la pestaña de Ocupantes activa (tab=2)
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
        Snackbar.Add("Ocupante creado exitosamente.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}