using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Personas;

public partial class PersonaSearch
{
    private List<Persona>? Personas { get; set; }
    private MudTable<Persona> tablePersonas = new();
    private int personasTotalRecords = 0;
    private bool loading;

    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsPersonasAsync();
    }

    private async Task LoadTotalRecordsPersonasAsync()
    {
        loading = true;
        var url = $"api/personas/totalRecords";

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

        personasTotalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<Persona>> LoadListPersonasAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"api/personas/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Persona>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Persona> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Persona> { Items = [], TotalItems = 0 };
        }
        return new TableData<Persona>
        {
            Items = responseHttp.Response,
            TotalItems = personasTotalRecords
        };
    }

    private async Task SetFilterValuePersonas(string value)
    {
        Filter = value;
        await LoadTotalRecordsPersonasAsync();
        await tablePersonas.ReloadServerData();
    }
}