using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.PersonalEncuestas;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.PersonalEncuestas;

public partial class PersonalEncuestasIndex
{
    private List<PersonalEncuesta>? PersonalEncuestas { get; set; }
    private MudTable<PersonalEncuesta> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/personalencuestas";
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

    private async Task<TableData<PersonalEncuesta>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<PersonalEncuesta>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<PersonalEncuesta> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<PersonalEncuesta> { Items = [], TotalItems = 0 };
        }
        return new TableData<PersonalEncuesta>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(PersonalEncuesta personalEncuesta)
    {
        NavigationManager.NavigateTo($"/personalEncuestas/details/{personalEncuesta.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private void RedirectToPersonalEncuestaForm(long id = 0, bool isEdit = false)
    {
        if (isEdit)
        {
            NavigationManager.NavigateTo($"/personalEncuesta/edit/{id}");
        }
        else
        {
            NavigationManager.NavigateTo($"/personalEncuesta/create");
        }
    }

    //private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    //{
    //    var options = new DialogOptions
    //    {
    //        CloseOnEscapeKey = true,
    //        CloseButton = true,
    //        MaxWidth = MaxWidth.Small,
    //        FullWidth = true,
    //        NoHeader = true
    //    };
    //    IDialogReference? dialog;
    //    if (isEdit)
    //    {
    //        var parameters = new DialogParameters
    //        {
    //            { "Id", id }
    //        }; dialog = await DialogService.ShowAsync<PersonalEncuestaEdit>("Editar Personal de Encuesta", parameters, options);
    //    }
    //    else
    //    {
    //        dialog = await DialogService.ShowAsync<PersonalEncuestaCreate>("Nuevo Personal de Encuesta", options);
    //    }

    //    var result = await dialog.Result;
    //    if (result!.Canceled!)
    //    {
    //        await LoadTotalRecordsAsync();
    //        await table.ReloadServerData();
    //    }
    //}

    private async Task DeleteAsync(PersonalEncuesta personalEncuesta)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"Estas seguro de borrar el Personal de Encuesta: {personalEncuesta.Persona!.NombreCompleto}" }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{personalEncuesta.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/personalEncuestas");
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