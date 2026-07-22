using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoCreate
{
    private DocumentoAnexo documentoAnexo = new();
    private DocumentoAnexoForm? documentoAnexoForm;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }
    [Parameter] public string CodEncuesta { get; set; } = string.Empty;

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

        // Actualizar el ID con el que devuelve el servidor
        var creado = responseHttp.Response as DocumentoAnexo;
        if (creado != null)
        {
            documentoAnexo.Id = creado.Id;
        }

        Snackbar.Add("Documento Anexo creado. Ahora puede agregar adjuntos.", Severity.Success);

        // Activar el modo adjuntos en el formulario
        documentoAnexoForm?.ActivarModoAdjuntos();
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=5");
    }
}