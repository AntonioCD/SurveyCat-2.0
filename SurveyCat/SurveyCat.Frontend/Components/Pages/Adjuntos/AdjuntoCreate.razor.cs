using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Adjuntos;

public partial class AdjuntoCreate
{
    private Adjunto adjunto = new();
    private IBrowserFile? selectedFile; // Guarda el archivo en memoria

    [Inject] private HttpClient Http { get; set; } = null!; // Agregamos HttpClient directo para multipart
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int DocumentoAnexoId { get; set; }
    [Parameter] public string CodEncuesta { get; set; } = null!;

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

        // Construimos el MultipartFormDataContent
        using var content = new MultipartFormDataContent();

        // 10MB máximo por archivo
        var streamContent = new StreamContent(selectedFile.OpenReadStream(1024 * 1024 * 10));
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

        // Agregamos el binario
        content.Add(streamContent, "archivo", adjunto.NombreArchivo);

        // Agregamos propiedades del modelo
        content.Add(new StringContent(adjunto.DocumentoAnexoId.ToString()), "DocumentoAnexoId");
        content.Add(new StringContent(adjunto.ItemPagina.ToString()), "ItemPagina");
        content.Add(new StringContent(adjunto.NombreArchivo), "NombreArchivo");
        content.Add(new StringContent(CodEncuesta), "CodEncuesta");

        // Hacemos el POST directo a la API usando multipart/form-data
        var response = await Http.PostAsync("api/adjuntos/cargar", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Error: {errorMsg}", Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/documentoAnexo/details/{DocumentoAnexoId}");
    }
}