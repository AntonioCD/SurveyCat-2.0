using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Adjuntos;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;
using static System.Net.WebRequestMethods;

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
    [Parameter] public int FichaId { get; set; }
    [Parameter] public bool IsEmbedded { get; set; } = false;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private HttpClient Http { get; set; } = null!;
    [Inject] private IMudDialogInstance MudDialog { get; set; } = null!;

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
                if (IsEmbedded)
                {
                    MudDialog.Cancel();
                }
                else
                {
                    NavigationManager.NavigateTo("/documentosAnexos");
                }
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            if (IsEmbedded)
            {
                MudDialog.Cancel();
            }
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
            loading = false;
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

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        if (IsEmbedded)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{FichaId}");
        }
    }

    private async Task ShowModalAsync()
    {
        if (documentoAnexo?.Ficha == null || string.IsNullOrWhiteSpace(documentoAnexo.Ficha.CodEncuesta))
        {
            Snackbar.Add(
                "No se puede agregar un adjunto porque el documento anexo no tiene un código de encuesta válido.",
                Severity.Warning);
            return;
        }

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var parameters = new DialogParameters
        {
            { "DocumentoAnexoId", DocumentoAnexoId },
            { "CodEncuesta", documentoAnexo!.Ficha!.CodEncuesta },
            { "IsEmbedded", true },
            { "FichaId", FichaId }
        };

        var dialog = await DialogService.ShowAsync<AdjuntoCreate>("Nuevo Adjunto", parameters, options);

        var result = await dialog.Result;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task VerDocumentoCompletoAsync(Adjunto adjunto)
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

    private void NoDepartamento()
    {
        if (IsEmbedded)
        {
            MudDialog.Cancel();
        }
        else
        {
            NavigationManager.NavigateTo("/documentosAnexos");
        }
    }
}