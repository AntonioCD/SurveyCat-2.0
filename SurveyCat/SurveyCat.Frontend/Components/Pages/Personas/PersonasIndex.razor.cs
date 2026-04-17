using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Personas
{
    public partial class PersonasIndex
    {
        private List<Persona>? Personas { get; set; }
        private MudTable<Persona> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/personas";
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

        private async Task<TableData<Persona>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

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
                TotalItems = totalRecords
            };
        }

        private void StatesAction(Persona persona)
        {
            NavigationManager.NavigateTo($"/personas/details/{persona.Id}");
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }

        private async Task ShowModalAsync(long id = 0, bool isEdit = false)
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };
            IDialogReference? dialog;
            if (isEdit)
            {
                var parameters = new DialogParameters
            {
                { "Id", id }
            }; dialog = await DialogService.ShowAsync<PersonaEdit>("Editar Persona", parameters, options);
            }
            else
            {
                dialog = await DialogService.ShowAsync<PersonaCreate>("Nueva Persona", options);
            }

            var result = await dialog.Result;
            if (result!.Canceled!)
            {
                await LoadTotalRecordsAsync();
                await table.ReloadServerData();
            }
        }

        private async Task DeleteAsync(Persona persona)
        {
            var parameters = new DialogParameters
        {
            { "Message", $"Estas seguro de borrar el persona: {persona.NombreCompleto}" }
        };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result!.Canceled)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{persona.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/personas");
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
}