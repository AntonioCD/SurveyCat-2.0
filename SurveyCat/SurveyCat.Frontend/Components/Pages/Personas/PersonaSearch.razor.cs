using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Personas;

public partial class PersonaSearch
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public bool SoloNaturales { get; set; }

    private List<Persona>? Personas { get; set; }
    private MudTable<Persona> tablePersonas = new();
    private int personasTotalRecords = 0;

    private string cedulaSearchTerm = string.Empty;
    private bool searchingCedula = false;
    private bool cedulaBuscadaNoEncontrada = false;
    private ResultadoBusquedaCedulaDTO? resultadoBusqueda;

    private string searchPrimerNombreTerm = string.Empty;
    private string searchSegundoNombreTerm = string.Empty;
    private string searchPrimerApellidoTerm = string.Empty;
    private string searchSegundoApellidoTerm = string.Empty;

    private bool searchingNombre = false;
    private ResultadoBusquedaNombreDTO? resultadoBusquedaNombre;

    private bool savingImport = false;
    private bool loading;

    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsPersonasAsync();
    }

    private void SeleccionarPersona(Persona persona)
    {
        MudDialog.Close(DialogResult.Ok(persona));
    }

    private void Cancelar()
    {
        MudDialog.Cancel();
    }

    private async Task LoadTotalRecordsPersonasAsync()
    {
        loading = true;

        try
        {
            var url = $"api/personas/totalRecordsPersonas?soloNaturales={SoloNaturales}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Uri.EscapeDataString(Filter)}";
            }

            var responseHttp = await Repository.GetAsync<int>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            personasTotalRecords = responseHttp.Response;
        }
        finally
        {
            loading = false;
        }
    }

    private async Task<TableData<Persona>> LoadListPersonasAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;

        var url = $"api/personas/paginatedPersonas?page={page}&recordsNumber={pageSize}&soloNaturales={SoloNaturales}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Uri.EscapeDataString(Filter)}";
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
                { "Id", id }
            };
            dialog = await DialogService.ShowAsync<PersonaEdit>("Editar Persona", parameters, options);
        }
        else
        {
            dialog = await DialogService.ShowAsync<PersonaCreate>("Nueva Persona", options);
        }

        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data != null)
        {
            await LoadTotalRecordsPersonasAsync();
            await tablePersonas.ReloadServerData();

            var mensaje = isEdit ? "Persona actualizada exitosamente" : "Persona creada exitosamente";
            Snackbar.Add(mensaje, Severity.Success);
        }
    }

    private async Task HandleCedulaKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await BuscarPorCedulaAsync();
        }
    }

    private async Task HandleNombreKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await BuscarPorNombreAsync();
        }
    }

    private async Task BuscarPorCedulaAsync()
    {
        if (string.IsNullOrWhiteSpace(cedulaSearchTerm))
        {
            Snackbar.Add("Ingrese un número de cédula para consultar.", Severity.Warning);
            return;
        }

        searchingCedula = true;
        cedulaBuscadaNoEncontrada = false;
        resultadoBusqueda = null;

        try
        {
            var responseHttp = await Repository.GetAsync<ResultadoBusquedaCedulaDTO>($"api/personas/buscar-cedula/{Uri.EscapeDataString(cedulaSearchTerm)}");

            if (responseHttp.Error)
            {
                var msg = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(msg!, Severity.Error);
                return;
            }

            resultadoBusqueda = responseHttp.Response;

            if (resultadoBusqueda != null)
            {
                if (resultadoBusqueda.EncontradoEnSistema && resultadoBusqueda.PersonaExistente != null)
                {
                    Snackbar.Add("La persona ya existe en el sistema. Seleccionada automáticamente.", Severity.Info);
                    SeleccionarPersona(resultadoBusqueda.PersonaExistente);
                }
                else if (!resultadoBusqueda.EncontradoEnPadron)
                {
                    cedulaBuscadaNoEncontrada = true;
                }
            }
        }
        finally
        {
            searchingCedula = false;
            StateHasChanged();
        }
    }

    private async Task BuscarPorNombreAsync()
    {
        if (string.IsNullOrWhiteSpace(searchPrimerNombreTerm) &&
            string.IsNullOrWhiteSpace(searchSegundoNombreTerm) &&
            string.IsNullOrWhiteSpace(searchPrimerApellidoTerm) &&
            string.IsNullOrWhiteSpace(searchSegundoApellidoTerm))
        {
            Snackbar.Add("Ingrese al menos un nombre o apellido para realizar la búsqueda.", Severity.Warning);
            return;
        }

        searchingNombre = true;
        resultadoBusquedaNombre = null;

        try
        {
            var url = $"api/personas/buscar-por-nombre" +
                      $"?primerNombre={Uri.EscapeDataString(searchPrimerNombreTerm)}" +
                      $"&segundoNombre={Uri.EscapeDataString(searchSegundoNombreTerm)}" +
                      $"&primerApellido={Uri.EscapeDataString(searchPrimerApellidoTerm)}" +
                      $"&segundoApellido={Uri.EscapeDataString(searchSegundoApellidoTerm)}";

            var responseHttp = await Repository.GetAsync<ResultadoBusquedaNombreDTO>(url);

            if (responseHttp.Error)
            {
                var msg = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(msg!, Severity.Error);
                return;
            }

            resultadoBusquedaNombre = responseHttp.Response;

            // Filtrar automáticamente la tabla general local con los valores ingresados
            var filtroCombinado = $"{searchPrimerNombreTerm} {searchSegundoNombreTerm} {searchPrimerApellidoTerm} {searchSegundoApellidoTerm}".Trim();
            if (!string.IsNullOrWhiteSpace(filtroCombinado))
            {
                await SetFilterValuePersonas(filtroCombinado);
            }
        }
        finally
        {
            searchingNombre = false;
            StateHasChanged();
        }
    }

    private async Task ImportarPersonaDesdePadronAsync(PadronPersonaDTO padronDTO)
    {
        if (padronDTO == null) return;

        savingImport = true;

        try
        {
            var responseHttp = await Repository.PostAsync<PadronPersonaDTO, Persona>("api/personas/importar-padron", padronDTO);

            if (responseHttp.Error)
            {
                var msg = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add($"Error al registrar la persona: {msg}", Severity.Error);
                return;
            }

            var nuevaPersona = responseHttp.Response;
            if (nuevaPersona != null)
            {
                Snackbar.Add("Persona agregada exitosamente al sistema desde el Padrón.", Severity.Success);
                SeleccionarPersona(nuevaPersona);
            }
        }
        finally
        {
            savingImport = false;
        }
    }
}