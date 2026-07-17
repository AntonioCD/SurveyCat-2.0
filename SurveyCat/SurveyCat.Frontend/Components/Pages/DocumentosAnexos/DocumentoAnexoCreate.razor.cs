using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoCreate
{
    private DocumentoAnexo documentoAnexo = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }
    [Parameter] public bool IsEmbedded { get; set; } = false;

    private async Task CreateAsync()
    {
        documentoAnexo.FichaId = FichaId;
        var responseHttp = await Repository.PostAsync<DocumentoAnexo>("/api/documentosAnexos", documentoAnexo);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Documento Anexo creado exitosamente.", Severity.Success);

        if (IsEmbedded)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{FichaId}");
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
            NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{FichaId}");
        }
    }
}