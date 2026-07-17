using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Fichas;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichasIndex
{
    private List<Ficha>? Fichas { get; set; }
    private MudTable<Ficha> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/fichas";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecords";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<Ficha>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Ficha>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Ficha> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Ficha> { Items = [], TotalItems = 0 };
        }
        return new TableData<Ficha>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void PropietariosAction(Ficha ficha)
    {
        //NavigationManager.NavigateTo($"/fichas/propietarios/details/{ficha.Id}");
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=1");
    }

    private void NucleoFamiliarAction(Ficha ficha)
    {
        //NavigationManager.NavigateTo($"/fichas/familias/details/{ficha.Id}");
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=2");
    }

    private void ColindantesAction(Ficha ficha)
    {
        //NavigationManager.NavigateTo($"/fichas/colindantes/details/{ficha.Id}");
        // Desde el index de fichas
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=3");
    }

    private void ConflictosAction(Ficha ficha)
    {
        //NavigationManager.NavigateTo($"/fichas/conflictos/details/{ficha.Id}");
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=4");
    }

    private void DocumentosAnexosAction(Ficha ficha)
    {
        //NavigationManager.NavigateTo($"/fichas/documentosAnexos/details/{ficha.Id}");
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=5");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private void RedirectToFichaForm(long id = 0, bool isEdit = false)
    {
        if (isEdit)
        {
            NavigationManager.NavigateTo($"/fichas/edit/{id}");
        }
        else
        {
            NavigationManager.NavigateTo($"/fichas/create");
        }
    }

    private async Task DeleteAsync(Ficha ficha)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"Estas seguro de borrar la ficha: {ficha.CodEncuesta}" }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{ficha.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/fichas");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add("Registro borrado", Severity.Success);
    }
}