using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Adjuntos;

public partial class AdjuntoCreate
{
    private Adjunto adjunto = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int DocumentoAnexoId { get; set; }

    private async Task CreateAsync()
    {
        adjunto.DocumentoAnexoId = DocumentoAnexoId;
        var responseHttp = await Repository.PostAsync("/api/adjuntos", adjunto);
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
        NavigationManager.NavigateTo($"/fichas/documentoAnexo/details/{DocumentoAnexoId}");
    }
}