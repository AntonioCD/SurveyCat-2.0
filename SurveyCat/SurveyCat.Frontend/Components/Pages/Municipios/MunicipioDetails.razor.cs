using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.BarriosComarcas;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Municipios;

public partial class MunicipioDetails
{
    private Municipio? municipio;
    private List<BarrioComarca>? barriosComarcas;

    private MudTable<BarrioComarca> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/barriosComarcas";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public int MunicipioId { get; set; }

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

    private async Task<bool> LoadMunicipioAsync()
    {
        var responseHttp = await Repository.GetAsync<Municipio>($"/api/municipios/{MunicipioId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/departamentos/details/{municipio!.DepartamentoId}");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        municipio = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;
        if (municipio is null)
        {
            var ok = await LoadMunicipioAsync();
            if (!ok)
            {
                NoMunicipio();
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={MunicipioId}";
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

    private async Task<TableData<BarrioComarca>> LoadListAsync(TableState barrioComarca, CancellationToken cancellationToken)
    {
        int page = barrioComarca.Page + 1;
        int pageSize = barrioComarca.PageSize;
        var url = $"{baseUrl}/paginated?id={MunicipioId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<BarrioComarca>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<BarrioComarca> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<BarrioComarca> { Items = [], TotalItems = 0 };
        }
        return new TableData<BarrioComarca>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(BarrioComarca barrioComarca)
    {
        NavigationManager.NavigateTo($"/barriosComarcas/details/{barrioComarca.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo($"/departamentos/details/{municipio!.DepartamentoId}");
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true
        };
        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
            {
                { "Id", id }
            }; dialog = await DialogService.ShowAsync<BarrioComarcaEdit>("Editar Barrio/Comarca", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
                {
                    { "MunicipioId", MunicipioId }
                };
            dialog = await DialogService.ShowAsync<BarrioComarcaCreate>("Nuevo Barrio/Comarca", parameters, options);
        }

        var result = await dialog.Result;
        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private void CaseriosAction(BarrioComarca barrioComarca)
    {
        NavigationManager.NavigateTo($"/barriosComarcas/details/{barrioComarca.Id}");
    }

    private void NoMunicipio()
    {
        NavigationManager.NavigateTo("/barriosComarcas");
    }

    private async Task DeleteAsync(BarrioComarca barrioComarca)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el BarrioComarca {barrioComarca.Nombre}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/barriosComarcas/{barrioComarca.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("BarrioComarca eliminado.", Severity.Success);
    }
}