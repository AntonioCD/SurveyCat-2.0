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

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<DocumentoAnexo>($"api/documentosAnexos/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("documentosAnexos");
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
        documentoAnexo!.Ficha = null;
        documentoAnexo.Documento = null;
        var responseHttp = await Repository.PutAsync("api/documentosAnexos", documentoAnexo);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro guardado.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{FichaId}");
    }
}