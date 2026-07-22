using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Conflictos;

public partial class ConflictoCreate
{
    private Conflicto conflicto = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }

    private async Task CreateAsync()
    {
        conflicto.FichaId = FichaId;
        var responseHttp = await Repository.PostAsync<Conflicto>("/api/conflictos", conflicto);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        // Primero el Snackbar, luego navegar
        Snackbar.Add("Conflicto creado exitosamente.", Severity.Success);
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=4");
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=4");
    }
}