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
        var responseHttp = await Repository.PostAsync("/api/conflictos", conflicto);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
    }
}