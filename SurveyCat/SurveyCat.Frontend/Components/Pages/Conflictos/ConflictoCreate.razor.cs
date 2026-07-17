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
    [Inject] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }
    [Parameter] public bool IsEmbedded { get; set; } = false;

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

        Snackbar.Add("Conflicto creado exitosamente.", Severity.Success);

        if (IsEmbedded)
        {
            // Si viene de la ficha, cerrar el diálogo y recargar
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            // Si es página independiente, navegar
            NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
        }
    }

    private void Return()
    {
        if (IsEmbedded)
        {
            MudDialog.Cancel();
        }
        else
        {
            NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
        }
    }
}