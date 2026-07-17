using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Adjuntos;

public partial class AdjuntoCreate
{
    private Adjunto adjunto = new();
    private IBrowserFile? selectedFile;

    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public int DocumentoAnexoId { get; set; }
    [Parameter] public string CodEncuesta { get; set; } = null!;
    [Parameter] public bool IsEmbedded { get; set; } = false;
    [Parameter] public int FichaId { get; set; }

    private void HandleFileSelected(IBrowserFile file)
    {
        selectedFile = file;
    }

    private async Task CreateAsync()
    {
        if (selectedFile == null)
        {
            Snackbar.Add("Debe seleccionar un archivo obligatoriamente.", Severity.Warning);
            return;
        }

        adjunto.DocumentoAnexoId = DocumentoAnexoId;

        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(selectedFile.OpenReadStream(1024 * 1024 * 10));
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

        content.Add(streamContent, "archivo", adjunto.NombreArchivo);
        content.Add(new StringContent(adjunto.DocumentoAnexoId.ToString()), "DocumentoAnexoId");
        content.Add(new StringContent(adjunto.ItemPagina.ToString()), "ItemPagina");
        content.Add(new StringContent(adjunto.NombreArchivo), "NombreArchivo");
        content.Add(new StringContent(CodEncuesta), "CodEncuesta");

        var response = await Http.PostAsync("api/adjuntos/cargar", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Error: {errorMsg}", Severity.Error);
            return;
        }

        Snackbar.Add("Adjunto creado exitosamente.", Severity.Success);

        if (IsEmbedded)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            NavigationManager.NavigateTo($"/documentoAnexo/details/{DocumentoAnexoId}");
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
            NavigationManager.NavigateTo($"/documentoAnexo/details/{DocumentoAnexoId}");
        }
    }
}