using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Propietarios;
using SurveyCat.Frontend.Components.Pages.Sectores;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaPropietariosDetails
{
    private Ficha? ficha;
    private List<Propietario>? propietarios;

    private MudTable<Propietario> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/propietarios";
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
                // No navegamos, solo retornamos false
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

    private async Task<TableData<Propietario>> LoadListAsync(TableState propietario, CancellationToken cancellationToken)
    {
        int page = propietario.Page + 1;
        int pageSize = propietario.PageSize;
        var url = $"{baseUrl}/paginated?id={FichaId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Propietario>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Propietario> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Propietario> { Items = [], TotalItems = 0 };
        }
        return new TableData<Propietario>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(Propietario propietario)
    {
        // Redirigimos dentro del contexto de la ficha
        NavigationManager.NavigateTo($"/propietarios/details/{propietario.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    // Ya no navegamos fuera, solo notificamos
    private void ReturnAction()
    {
        // No hacemos nada, estamos dentro de la ficha
    }

    private void RedirectToPropietarioForm(long id = 0, bool isEdit = false)
    {
        if (isEdit)
        {
            NavigationManager.NavigateTo($"/propietarios/edit/{id}/{FichaId}");
        }
        else
        {
            NavigationManager.NavigateTo($"/propietarios/create/{FichaId}");
        }
    }

    private void CaseriosAction(Propietario propietario)
    {
        NavigationManager.NavigateTo($"/propietarios/details/{propietario.Id}");
    }

    private void NoFicha()
    {
        // Mostramos un mensaje en lugar de navegar
        Snackbar.Add("No se encontró la ficha asociada.", Severity.Warning);
    }

    private async Task DeleteAsync(Propietario propietario)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Propietario {propietario.Persona?.NombreCompleto}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/propietarios/{propietario.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("Propietario eliminado.", Severity.Success);

        //// Notificar cambio
        //await OnPropietarioChanged.InvokeAsync();
    }
}