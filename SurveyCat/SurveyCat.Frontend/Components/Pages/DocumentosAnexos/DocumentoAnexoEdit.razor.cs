using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoEdit
{
    private DocumentoAnexo? documentoAnexo;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public long Id { get; set; }
    [Parameter] public int FichaId { get; set; }
    [Parameter] public string CodEncuesta { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<DocumentoAnexo>($"api/documentosAnexos/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=5");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            documentoAnexo = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        if (documentoAnexo == null) return;

        // Limpiar propiedades de navegación antes de enviar
        documentoAnexo.Ficha = null;
        documentoAnexo.Documento = null;

        var responseHttp = await Repository.PutAsync<DocumentoAnexo>("api/documentosAnexos", documentoAnexo);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Snackbar.Add("Documento Anexo guardado exitosamente.", Severity.Success);
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=5");
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=5");
    }
}