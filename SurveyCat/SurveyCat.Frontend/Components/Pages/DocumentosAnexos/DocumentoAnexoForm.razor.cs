using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaDocumentos = new();

    private Diccionario? selectedDocumento = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public DocumentoAnexo DocumentoAnexo { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (DocumentoAnexo == null)
        {
            DocumentoAnexo = new DocumentoAnexo();
        }

        // Recrear el EditContext si el modelo cambió
        if (editContext == null || editContext.Model != DocumentoAnexo)
        {
            editContext = new EditContext(DocumentoAnexo);
        }

        await LoadDiccionariosAsync();

        if (DocumentoAnexo.Id != 0)
        {
            selectedDocumento = listaDocumentos.Where(x => x.Id == DocumentoAnexo.DocumentoId).FirstOrDefault();
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
            listaDocumentos = diccionarios.Where(x => x.Catalogo == Catalogos.Documento).ToList();
        }
    }

    private void DocumentoChanged(Diccionario documento)
    {
        if (documento != null)
        {
            selectedDocumento = documento;
            DocumentoAnexo.DocumentoId = documento!.Id;
        }
    }

    private async Task<IEnumerable<Diccionario>> SearchDocumento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaDocumentos!;
        }

        return listaDocumentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
}