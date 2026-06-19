using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Adjuntos;

public partial class AdjuntoForm
{
    private EditContext editContext = null!;
    private DocumentoAnexo? documentoAnexo = new();

    // Almacena la extensión original (.pdf, .jpg, etc.) y el nombre original para la UI
    private string ArchivoOriginalName { get; set; } = string.Empty;

    private string Extension { get; set; } = string.Empty;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Parameter] public int DocumentoAnexoId { get; set; }
    [EditorRequired, Parameter] public Adjunto Adjunto { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Adjunto == null)
        {
            Adjunto = new Adjunto();
        }

        if (editContext == null || editContext.Model != Adjunto)
        {
            editContext = new EditContext(Adjunto);
        }

        await LoadDocumentoAnexoAsync();
        GenerarNombreArchivo(); // Calcula el nombre si ya existen datos precargados
    }

    protected override void OnInitialized()
    {
        editContext = new EditContext(Adjunto);
    }

    private async Task LoadDocumentoAnexoAsync()
    {
        var responseHttp = await Repository.GetAsync<DocumentoAnexo>($"api/documentosAnexos/{DocumentoAnexoId}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        documentoAnexo = responseHttp.Response;
        GenerarNombreArchivo();
    }

    // Intercepta el cambio del campo numérico para recalcular el nombre al instante
    private void OnItemPaginaChanged(int value)
    {
        // Verificamos si ya tenemos la información del documento anexo y sus páginas totales
        if (documentoAnexo != null && documentoAnexo.NumeroPaginas > 0)
        {
            if (value > documentoAnexo.NumeroPaginas)
            {
                // Si el usuario digita un número mayor, lo limitamos al máximo permitido
                Adjunto.ItemPagina = documentoAnexo.NumeroPaginas;

                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
                Snackbar.Add($"La página no puede ser mayor al total de páginas del documento ({documentoAnexo.NumeroPaginas}).", Severity.Warning);

                GenerarNombreArchivo();
                return;
            }
        }

        // Si pasa la validación, asignamos el valor normal
        Adjunto.ItemPagina = value;
        GenerarNombreArchivo();
    }

    private void UploadFileAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            ArchivoOriginalName = file.Name;
            Extension = Path.GetExtension(file.Name); // Extrae la extensión (.pdf)

            GenerarNombreArchivo();

            // Seteamos una ruta relativa simulada con el nombre final calculado
            Adjunto.Ruta = $"uploads/docs/{Adjunto.NombreArchivo}";
        }
    }

    // LÓGICA CORE: Genera el string dinámico
    private void GenerarNombreArchivo()
    {
        if (documentoAnexo?.Documento != null)
        {
            var codigo = documentoAnexo.Documento.Codigo ?? string.Empty;
            var numPaginas = documentoAnexo.NumeroPaginas;

            // :D2 convierte el número 1 en "01", el 9 en "09", manteniendo intactos números superiores (ej: 12)
            var itemPaginaStr = Adjunto.ItemPagina.ToString("D2");

            // Construye la cadena base: Codigo + NumeroPaginas + ItemPagina padded
            Adjunto.NombreArchivo = $"{codigo}{numPaginas}{itemPaginaStr}{Extension}";

            // Notifica al EditContext el cambio del valor para limpiar validaciones pendientes
            editContext.NotifyFieldChanged(FieldIdentifier.Create(() => Adjunto.NombreArchivo));
        }
    }
}