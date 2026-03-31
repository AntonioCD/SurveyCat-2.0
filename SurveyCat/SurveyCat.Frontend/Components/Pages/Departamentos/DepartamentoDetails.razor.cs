using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Municipios;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Departamentos;

public partial class DepartamentoDetails
{
    private Departamento? departamento;
    private List<Municipio>? municipios;

    private MudTable<Municipio> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/municipios";
    private string infoFormat = "{first_item}-{last_item} de {all_items}";

    [Parameter] public int DepartamentoId { get; set; }

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

    private async Task<bool> LoadDepartamentoAsync()
    {
        var responseHttp = await Repository.GetAsync<Departamento>($"/api/departamentos/{DepartamentoId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/departamentos");
                return false;
            }

            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }
        departamento = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;
        if (departamento is null)
        {
            var ok = await LoadDepartamentoAsync();
            if (!ok)
            {
                NoDepartamento();
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={DepartamentoId}";
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

    private async Task<TableData<Municipio>> LoadListAsync(TableState municipio, CancellationToken cancellationToken)
    {
        int page = municipio.Page + 1;
        int pageSize = municipio.PageSize;
        var url = $"{baseUrl}/paginated?id={DepartamentoId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Municipio>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Municipio> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Municipio> { Items = [], TotalItems = 0 };
        }
        return new TableData<Municipio>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(Municipio municipio)
    {
        NavigationManager.NavigateTo($"/municipios/details/{municipio.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/departamentos");
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
            }; dialog = await DialogService.ShowAsync<MunicipioEdit>("Editar Municipio", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
                {
                    { "DepartamentoId", DepartamentoId }
                };
            dialog = await DialogService.ShowAsync<MunicipioCreate>("Nuevo Municipio", parameters, options);
        }

        var result = await dialog.Result;
        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private void BarriosComarcasAction(Municipio municipio)
    {
        NavigationManager.NavigateTo($"/municipios/details/{municipio.Id}");
    }

    private void NoDepartamento()
    {
        NavigationManager.NavigateTo("/departamentos");
    }

    private async Task DeleteAsync(Municipio municipio)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Municipio {municipio.Nombre}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/municipios/{municipio.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("Municipio eliminado.", Severity.Success);
    }
}