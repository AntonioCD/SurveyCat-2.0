using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Caserios;

public partial class CaserioCreate
{
    private Caserio caserio = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int BarrioComarcaId { get; set; }

    private async Task CreateAsync()
    {
        caserio.ComarcaId = BarrioComarcaId;
        var responseHttp = await Repository.PostAsync("/api/caserios", caserio);
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
        NavigationManager.NavigateTo($"/barriosComarcas/details/{BarrioComarcaId}");
    }
}