using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Familias;

public partial class FamiliaCreate
{
    private Familia familia = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }

    private async Task CreateAsync()
    {
        familia.FichaId = FichaId;
        var responseHttp = await Repository.PostAsync<Familia>("/api/familias", familia);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        // Volver a la ficha con la pestaña de Núcleo Familiar activa (tab=2)
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
        Snackbar.Add("Familiar creado exitosamente.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}