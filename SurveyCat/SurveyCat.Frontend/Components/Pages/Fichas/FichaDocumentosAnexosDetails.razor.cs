using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Conflictos;
using SurveyCat.Frontend.Components.Pages.DocumentosAnexos;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaDocumentosAnexosDetails
{
    private Ficha? ficha;
    private List<DocumentoAnexo>? documentosAnexos;

    private MudTable<DocumentoAnexo> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/documentosAnexos";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public long FichaId { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task<bool> LoadFichaAsync()
    {
        var responseHttp = await Repository.GetAsync<Ficha>($"/api/fichas/{FichaId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        ficha = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;
        if (ficha is null)
        {
            var ok = await LoadFichaAsync();
            if (!ok)
            {
                NoFicha();
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={FichaId}";
        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }
        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        totalRecords = responseHttp.Response;
        loading = false;
        return true;
    }

    private async Task<TableData<DocumentoAnexo>> LoadListAsync(TableState documentoAnexo, CancellationToken cancellationToken)
    {
        int page = documentoAnexo.Page + 1;
        int pageSize = documentoAnexo.PageSize;
        var url = $"{baseUrl}/paginated?id={FichaId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<DocumentoAnexo>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<DocumentoAnexo> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<DocumentoAnexo> { Items = [], TotalItems = 0 };
        }
        return new TableData<DocumentoAnexo>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void ShowModalAsync(long id = 0, bool isEdit = false)
    {
        if (isEdit)
        {
            NavigationManager.NavigateTo($"/documentosAnexos/edit/{id}/{FichaId}/{ficha!.CodEncuesta}");
        }
        else
        {
            NavigationManager.NavigateTo($"/documentosAnexos/create/{FichaId}/{ficha!.CodEncuesta}");
        }
    }

    private void AdjuntosAction(DocumentoAnexo documentoAnexo)
    {
        // Navegar dentro del contexto de la ficha
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=5");
        // Abrir el diálogo de adjuntos - Podrías modificar esto para usar un diálogo en lugar de navegar
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void NoFicha()
    {
        NavigationManager.NavigateTo("/fichas");
    }

    private async Task DeleteAsync(DocumentoAnexo documentoAnexo)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Documento Anexo {documentoAnexo.Documento?.Nombre}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/documentosAnexos/{documentoAnexo.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("DocumentoAnexo eliminado.", Severity.Success);
    }
}