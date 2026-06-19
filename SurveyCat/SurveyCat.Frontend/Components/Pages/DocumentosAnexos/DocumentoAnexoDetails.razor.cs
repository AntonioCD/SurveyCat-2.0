using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Adjuntos;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.DocumentosAnexos;

public partial class DocumentoAnexoDetails
{
    private DocumentoAnexo? documentoAnexo;
    private List<Adjunto>? adjuntos;

    private MudTable<Adjunto> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/adjuntos";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public int DocumentoAnexoId { get; set; }

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

    private async Task<bool> LoadDocumentoAnexoAsync()
    {
        var responseHttp = await Repository.GetAsync<DocumentoAnexo>($"/api/documentosAnexos/{DocumentoAnexoId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/documentosAnexos");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        documentoAnexo = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;
        if (documentoAnexo is null)
        {
            var ok = await LoadDocumentoAnexoAsync();
            if (!ok)
            {
                NoDepartamento();
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={DocumentoAnexoId}";
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

    private async Task<TableData<Adjunto>> LoadListAsync(TableState adjunto, CancellationToken cancellationToken)
    {
        int page = adjunto.Page + 1;
        int pageSize = adjunto.PageSize;
        var url = $"{baseUrl}/paginated?id={DocumentoAnexoId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Adjunto>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Adjunto> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Adjunto> { Items = [], TotalItems = 0 };
        }
        return new TableData<Adjunto>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(Adjunto adjunto)
    {
        NavigationManager.NavigateTo($"/adjuntos/details/{adjunto.Id}");
    }

    private void SectoresAction(Adjunto adjunto)
    {
        NavigationManager.NavigateTo($"/adjuntos/sectores/details/{adjunto.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{DocumentoAnexoId}");
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            NoHeader = true
        };
        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
            {
                { "Id", id }
            }; dialog = await DialogService.ShowAsync<AdjuntoEdit>("Editar Adjunto", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
                {
                    { "DocumentoAnexoId", DocumentoAnexoId }
                };
            dialog = await DialogService.ShowAsync<AdjuntoCreate>("Nuevo Adjunto", parameters, options);
        }

        var result = await dialog.Result;
        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private void BarriosComarcasAction(Adjunto adjunto)
    {
        NavigationManager.NavigateTo($"/adjuntos/details/{adjunto.Id}");
    }

    private void NoDepartamento()
    {
        NavigationManager.NavigateTo("/documentosAnexos");
    }

    private async Task DeleteAsync(Adjunto adjunto)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Adjunto {adjunto.NombreArchivo}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/adjuntos/{adjunto.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("Adjunto eliminado.", Severity.Success);
    }
}