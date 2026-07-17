using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Conflictos;
using SurveyCat.Frontend.Components.Pages.PersonalEncuestas;
using SurveyCat.Frontend.Components.Pages.Sectores;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaConflictosDetails
{
    private Ficha? ficha;
    private List<Conflicto>? conflictos;

    private MudTable<Conflicto> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/conflictos";
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
                NavigationManager.NavigateTo($"/fichas");
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

    private async Task<TableData<Conflicto>> LoadListAsync(TableState conflicto, CancellationToken cancellationToken)
    {
        int page = conflicto.Page + 1;
        int pageSize = conflicto.PageSize;
        var url = $"{baseUrl}/paginated?id={FichaId}&page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Conflicto>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<Conflicto> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Conflicto> { Items = [], TotalItems = 0 };
        }
        return new TableData<Conflicto>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private void StatesAction(Conflicto conflicto)
    {
        NavigationManager.NavigateTo($"/conflictos/details/{conflicto.Id}");
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
        await table.ReloadServerData();
    }

    // El método ShowModalAsync permanece igual, pero los diálogos ahora cerrarán y volverán a la ficha
    private async Task ShowModalAsync(long id = 0, bool isEdit = false)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
        {
            { "Id", id },
            { "FichaId", FichaId },
            { "IsEmbedded", true } // Nuevo parámetro para saber que viene de la ficha
        };
            dialog = await DialogService.ShowAsync<ConflictoEdit>("Editar Conflicto", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
        {
            { "FichaId", FichaId },
            { "IsEmbedded", true } // Nuevo parámetro para saber que viene de la ficha
        };
            dialog = await DialogService.ShowAsync<ConflictoCreate>("Nuevo Conflicto", parameters, options);
        }

        var result = await dialog.Result;
        // Siempre recargar la tabla cuando se cierra el diálogo
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    //private async Task ShowModalAsync(long id = 0, bool isEdit = false)
    //{
    //    var options = new DialogOptions
    //    {
    //        CloseOnEscapeKey = true,
    //        CloseButton = true,
    //        NoHeader = true,
    //    };
    //    IDialogReference? dialog;
    //    if (isEdit)
    //    {
    //        var parameters = new DialogParameters
    //        {
    //            { "Id", id },
    //            { "FichaId", FichaId }
    //        }; dialog = await DialogService.ShowAsync<ConflictoEdit>("Editar Conflicto", parameters, options);
    //    }
    //    else
    //    {
    //        var parameters = new DialogParameters
    //            {
    //                { "FichaId", FichaId }
    //            };
    //        dialog = await DialogService.ShowAsync<ConflictoCreate>("Nuevo Conflicto", parameters, options);
    //    }

    //    var result = await dialog.Result;
    //    if (result!.Canceled!)
    //    {
    //        await LoadTotalRecordsAsync();
    //        await table.ReloadServerData();
    //    }
    //}

    private void CaseriosAction(Conflicto conflicto)
    {
        NavigationManager.NavigateTo($"/conflictos/details/{conflicto.Id}");
    }

    private void NoFicha()
    {
        NavigationManager.NavigateTo("/fichas");
    }

    private async Task DeleteAsync(Conflicto conflicto)
    {
        var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el Conflicto {conflicto.TipoConflicto?.Nombre}?" }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/conflictos/{conflicto.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadAsync();
        await table.ReloadServerData();
        Snackbar.Add("Conflicto eliminado.", Severity.Success);
    }
}