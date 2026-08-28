using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Adjuntos;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoForm
{
    private EditContext editContext = null!;
    private bool loading = true;
    private bool isInitialized = false;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaDocumentos = new();

    private Diccionario? selectedDocumento;

    // Estados para el flujo unificado
    private bool documentoGuardado = false;

    private bool mostrandoFormularioAdjunto = false;
    private List<Adjunto> adjuntos = new();
    private Adjunto nuevoAdjunto = new();

    // Selección de carpeta y archivo en 2 Pasos
    private string CarpetaSeleccionada = string.Empty;
    private List<IBrowserFile> archivosEnCarpeta = new();
    private IBrowserFile? selectedFile;
    private string ArchivoOriginalName = string.Empty;
    private string Extension = string.Empty;

    private bool adjuntoListoParaGuardar => selectedFile != null && nuevoAdjunto.ItemPagina > 0;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter] public string CodEncuesta { get; set; } = string.Empty;

    [EditorRequired, Parameter] public DocumentoAnexo DocumentoAnexo { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnParametersSet()
    {
        if (isInitialized)
            return;

        if (DocumentoAnexo == null)
        {
            DocumentoAnexo = new DocumentoAnexo();
        }

        if (editContext == null || editContext.Model != DocumentoAnexo)
        {
            editContext = new EditContext(DocumentoAnexo);
        }

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        loading = true;

        try
        {
            await LoadDiccionariosAsync();

            if (DocumentoAnexo.Id != 0)
            {
                selectedDocumento = listaDocumentos
                    .FirstOrDefault(x => x.Id == DocumentoAnexo.DocumentoId);

                documentoGuardado = true;
                await CargarAdjuntosAsync();
            }
            else
            {
                selectedDocumento = null;
            }
        }
        finally
        {
            loading = false;
            isInitialized = true;
            StateHasChanged();
        }
    }

    private async Task LoadDiccionariosAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Diccionario>>("/api/diccionarios/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        diccionarios = responseHttp.Response;

        if (diccionarios != null)
        {
            listaDocumentos = diccionarios
                .Where(x => x.Catalogo == Catalogos.Documento)
                .ToList();
        }
    }

    private async Task CargarAdjuntosAsync()
    {
        var url = $"api/adjuntos/paginated?id={DocumentoAnexo.Id}&page=1&recordsnumber=100";
        var responseHttp = await Repository.GetAsync<List<Adjunto>>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        adjuntos = responseHttp.Response ?? new List<Adjunto>();
    }

    private void DocumentoChanged(Diccionario documento)
    {
        selectedDocumento = documento;
        DocumentoAnexo.DocumentoId = documento?.Id ?? 0;
    }

    private async Task<IEnumerable<Diccionario>> SearchDocumento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaDocumentos!;

        return listaDocumentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || c.Codigo!.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    // ================= SECCIÓN 2: ADJUNTOS (FLUJO DE 2 PASOS) =================

    private void AbrirFormularioAdjunto()
    {
        nuevoAdjunto = new Adjunto
        {
            DocumentoAnexoId = DocumentoAnexo.Id,
            ItemPagina = 1
        };
        LimpiarSeleccionCarpeta();
        mostrandoFormularioAdjunto = true;
    }

    private void CancelarAdjunto()
    {
        mostrandoFormularioAdjunto = false;
        nuevoAdjunto = new Adjunto();
        LimpiarSeleccionCarpeta();
    }

    private async Task AbrirSeleccionCarpeta()
    {
        await JSRuntime.InvokeVoidAsync("openFolderPicker", "fileFolderInput");
    }

    private async Task UploadFolderAsync(InputFileChangeEventArgs e)
    {
        try
        {
            var files = e.GetMultipleFiles(maximumFileCount: 500).ToList();
            if (!files.Any()) return;

            var relativePath = await JSRuntime.InvokeAsync<string>("getFolderRelativePath", "fileFolderInput");
            var pathParts = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var nombreCarpeta = pathParts.Length > 0 ? pathParts[0] : string.Empty;

            // VALIDACIÓN: Verificar si el nombre de la carpeta coincide con el código de la encuesta
            if (!string.IsNullOrEmpty(CodEncuesta) && !nombreCarpeta.Equals(CodEncuesta, StringComparison.OrdinalIgnoreCase))
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
                Snackbar.Add($"La carpeta seleccionada '{nombreCarpeta}' NO coincide con el código de encuesta '{CodEncuesta}'.", Severity.Error);

                LimpiarSeleccionCarpeta();
                return;
            }

            CarpetaSeleccionada = nombreCarpeta;
            archivosEnCarpeta = files;

            // Preselecciona el primer archivo encontrado
            var primerArchivo = archivosEnCarpeta.FirstOrDefault();
            if (primerArchivo != null)
            {
                OnFileSelectedFromFolder(primerArchivo);
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al leer la carpeta: {ex.Message}", Severity.Error);
        }
    }

    private void OnFileSelectedFromFolder(IBrowserFile file)
    {
        selectedFile = file;
        if (selectedFile != null)
        {
            ArchivoOriginalName = selectedFile.Name;
            Extension = Path.GetExtension(selectedFile.Name);
            GenerarNombreArchivoAdjunto();
        }
    }

    private void LimpiarSeleccionCarpeta()
    {
        CarpetaSeleccionada = string.Empty;
        archivosEnCarpeta.Clear();
        selectedFile = null;
        ArchivoOriginalName = string.Empty;
        Extension = string.Empty;
        nuevoAdjunto.NombreArchivo = string.Empty;
        StateHasChanged();
    }

    private void GenerarNombreArchivoAdjunto()
    {
        if (nuevoAdjunto.ItemPagina > DocumentoAnexo.NumeroPaginas)
        {
            nuevoAdjunto.ItemPagina = DocumentoAnexo.NumeroPaginas;
            Snackbar.Add($"La página no puede ser mayor al total de páginas ({DocumentoAnexo.NumeroPaginas}).", Severity.Warning);
        }
        else if (nuevoAdjunto.ItemPagina < 1)
        {
            nuevoAdjunto.ItemPagina = 1;
        }

        if (selectedDocumento != null && !string.IsNullOrEmpty(Extension))
        {
            var codigo = selectedDocumento.Codigo ?? string.Empty;
            var numPaginas = DocumentoAnexo.NumeroPaginas.ToString("D2");
            var itemPaginaStr = nuevoAdjunto.ItemPagina.ToString("D2");
            nuevoAdjunto.NombreArchivo = $"{codigo}{numPaginas}{itemPaginaStr}{Extension}";
        }
    }

    private async Task GuardarAdjuntoAsync()
    {
        if (selectedFile == null)
        {
            Snackbar.Add("Debe seleccionar un archivo de la carpeta.", Severity.Warning);
            return;
        }

        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(selectedFile.OpenReadStream(1024 * 1024 * 10));
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

        content.Add(streamContent, "archivo", nuevoAdjunto.NombreArchivo);
        content.Add(new StringContent(nuevoAdjunto.DocumentoAnexoId.ToString()), "DocumentoAnexoId");
        content.Add(new StringContent(nuevoAdjunto.ItemPagina.ToString()), "ItemPagina");
        content.Add(new StringContent(nuevoAdjunto.NombreArchivo), "NombreArchivo");
        content.Add(new StringContent(CodEncuesta), "CodEncuesta");

        var response = await Http.PostAsync("api/adjuntos/cargar", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Error al subir archivo: {errorMsg}", Severity.Error);
            return;
        }

        Snackbar.Add("Adjunto creado exitosamente.", Severity.Success);
        CancelarAdjunto();
        await CargarAdjuntosAsync();
        StateHasChanged();
    }

    public void ActivarModoAdjuntos()
    {
        documentoGuardado = true;
        StateHasChanged();
    }

    private async Task VerDocumentoAsync(Adjunto adjunto)
    {
        var parameters = new DialogParameters();
        var apiBaseUrl = Http.BaseAddress?.ToString().TrimEnd('/') ?? "";
        var rutaLimpia = adjunto.Ruta.TrimStart('/');
        var urlCompleta = $"{apiBaseUrl}/{rutaLimpia}";

        parameters.Add("DocumentoUrl", urlCompleta);
        parameters.Add("NombreArchivo", adjunto.NombreArchivo);

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        await DialogService.ShowAsync<AdjuntoVisor>(adjunto.NombreArchivo, parameters, options);
    }

    private async Task EliminarAdjuntoAsync(Adjunto adjunto)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"¿Estás seguro de que quieres eliminar el adjunto {adjunto.NombreArchivo}?" }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;

        if (result!.Canceled) return;

        var responseHttp = await Repository.DeleteAsync($"api/adjuntos/{adjunto.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Adjunto eliminado.", Severity.Success);
        await CargarAdjuntosAsync();
        StateHasChanged();
    }

    private void Finalizar()
    {
        ReturnAction.InvokeAsync();
    }
}