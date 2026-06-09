using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Caserios;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.BarriosComarcas;

public partial class BarrioComarcaDetails
{
    private BarrioComarca? barrioComarca;
    private List<Caserio>? caserios;

    private MudTable<Caserio> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/caserios";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public int BarrioComarcaId { get; set; }

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

    private async Task<bool> LoadBarrioComarcaAsync()
    {
        var responseHttp = await Repository.GetAsync<BarrioComarca>($"/api/barriosComarcas/{BarrioComarcaId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/municipios/details/{barrioComarca!.MunicipioId}");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        barrioComarca = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;
        if (barrioComarca is null)
        {
            var ok = await LoadBarrioComarcaAsync();
            if (!ok)
            {
                NoBarrioComarca();
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={BarrioComarcaId}";
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

    private async Task<TableData<Caserio>> LoadListAsync(TableState caserio, CancellationToken cancellationToken)
    {
        int page = caserio.Page + 1;
        int pageSize = caserio.PageSize;
        var url = $"{baseUrl}/paginated?id={BarrioComarcaId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Caserio>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Caserio> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Caserio> { Items = [], TotalItems = 0 };
        }
        return new TableData<Caserio>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(Caserio caserio)
    {
        NavigationManager.NavigateTo($"/caserios/details/{caserio.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo($"/municipios/details/{barrioComarca!.MunicipioId}");
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
            }; dialog = await DialogService.ShowAsync<CaserioEdit>("Editar Caserio", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
                {
                    { "BarrioComarcaId", BarrioComarcaId }
                };
            dialog = await DialogService.ShowAsync<CaserioCreate>("Nuevo Caserio", parameters, options);
        }

        var result = await dialog.Result;
        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private void CaseriosAction(Caserio caserio)
    {
        NavigationManager.NavigateTo($"/caserios/details/{caserio.Id}");
    }

    private void NoBarrioComarca()
    {
        NavigationManager.NavigateTo("/caserios");
    }

    private async Task DeleteAsync(Caserio caserio)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Caserio {caserio.Nombre}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/caserios/{caserio.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("Caserio eliminado.", Severity.Success);
    }
}