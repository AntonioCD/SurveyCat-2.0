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
    private bool _reloadPending = false;

    private List<Jornada>? jornadas;
    private Jornada? selectedJornada;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadJornadasAsync();
        await LoadTotalRecordsAsync();
    }

    private async Task LoadJornadasAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Jornada>>("/api/jornadas/combo");
        if (!responseHttp.Error && responseHttp.Response != null)
        {
            jornadas = responseHttp.Response;
        }
    }

    private string FormatJornada(Jornada? jornada)
    {
        if (jornada == null)
            return string.Empty;

        return $"{jornada.CodJornada} - {jornada.Nombre}";
    }

    private async Task<IEnumerable<Jornada>> SearchJornada(string searchText, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (jornadas == null)
            return Enumerable.Empty<Jornada>();

        if (string.IsNullOrWhiteSpace(searchText))
            return jornadas;

        return jornadas.Where(c =>
            (c.Nombre != null && c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) ||
            (c.CodJornada != null && c.CodJornada.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    private async Task JornadaFilterChangedAsync(Jornada? jornada)
    {
        // 1. Asignar la jornada seleccionada o nula
        selectedJornada = jornada;

        // 2. Dar tiempo a JavaScript / MudBlazor para que destruya de forma segura el Popover desplegado
        await Task.Delay(1);

        // 3. Cargar el total de registros
        await LoadTotalRecordsAsync();

        // 4. Recargar la tabla garantizando que no sea nula
        if (table != null)
        {
            await table.ReloadServerData();
        }
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecords?";

        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            queryParams.Add($"filter={Uri.EscapeDataString(Filter)}");
        }

        if (selectedJornada != null && selectedJornada.Id > 0)
        {
            queryParams.Add($"id={selectedJornada.Id}");
        }

        url += string.Join("&", queryParams);

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            loading = false;
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
            url += $"&filter={Uri.EscapeDataString(Filter)}";
        }

        if (selectedJornada != null && selectedJornada.Id > 0)
        {
            url += $"&id={selectedJornada.Id}";
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
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=1");
    }

    private void OcupantesAction(Ficha ficha)
    {
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=2");
    }

    private void ColindantesAction(Ficha ficha)
    {
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=3");
    }

    private void ConflictosAction(Ficha ficha)
    {
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=4");
    }

    private void DocumentosAnexosAction(Ficha ficha)
    {
        NavigationManager.NavigateTo($"/fichas/edit/{ficha.Id}?tab=5");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();

        if (table != null)
        {
            await Task.Yield();
            await table.ReloadServerData();
        }
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

        if (table != null)
        {
            await table.ReloadServerData();
        }

        Snackbar.Add("Registro borrado", Severity.Success);
    }
}