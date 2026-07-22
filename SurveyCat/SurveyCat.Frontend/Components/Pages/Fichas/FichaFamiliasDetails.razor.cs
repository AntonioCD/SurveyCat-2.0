using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Shared;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaFamiliasDetails
{
    private Ficha? ficha;
    private List<Familia> familias = new();
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/familias";

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
        loading = true;
        StateHasChanged(); // Mostrar loading inmediatamente

        try
        {
            // 1. Cargamos la Ficha si no existe
            if (ficha is null)
            {
                var ok = await LoadFichaAsync();
                if (!ok)
                {
                    NoFicha();
                    return;
                }
            }

            // 2. Traemos la lista completa del backend
            var urlList = $"{baseUrl}/paginated?id={FichaId}&page=1&recordsnumber=100";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                urlList += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<Familia>>(urlList);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                familias = new List<Familia>();
            }
            else if (responseHttp.Response != null)
            {
                familias = responseHttp.Response.OrderBy(f => f.Item).ToList();
                totalRecords = familias.Count;
            }
        }
        finally
        {
            loading = false;
            StateHasChanged(); // Forzar re-renderizado al terminar
        }
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

    private async Task OnItemDropped(MudItemDropInfo<Familia> dropInfo)
    {
        if (dropInfo.Item == null || string.IsNullOrEmpty(dropInfo.DropzoneIdentifier)) return;
        if (!int.TryParse(dropInfo.DropzoneIdentifier, out int itemDestino)) return;

        var itemMovido = dropInfo.Item;
        if (itemMovido.Item == itemDestino) return;

        var listaModificada = familias.Where(f => f.Id != itemMovido.Id).OrderBy(f => f.Item).ToList();

        int nuevoIndice = itemDestino - 1;
        if (nuevoIndice < 0) nuevoIndice = 0;
        if (nuevoIndice > listaModificada.Count) nuevoIndice = listaModificada.Count;

        listaModificada.Insert(nuevoIndice, itemMovido);

        var listaParaEnviar = listaModificada.Select((f, index) => new Familia
        {
            Id = f.Id,
            FichaId = f.FichaId,
            Item = index + 1,
            PersonaId = f.PersonaId,
            Persona = f.Persona,
            ParentescoId = f.ParentescoId,
            Parentesco = f.Parentesco
        }).ToList();

        familias = listaParaEnviar;
        StateHasChanged();

        var responseHttp = await Repository.PostAsync($"{baseUrl}/reorder", familias);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add($"Error al guardar el nuevo orden: {message}", Severity.Error);
            await LoadAsync();
        }
        else
        {
            Snackbar.Add("Orden familiar actualizado con éxito.", Severity.Success);
        }
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadAsync();
    }

    private void RedirectToFamiliarForm(long id = 0, bool isEdit = false)
    {
        if (isEdit)
        {
            NavigationManager.NavigateTo($"/familias/edit/{id}/{FichaId}");
        }
        else
        {
            NavigationManager.NavigateTo($"/familias/create/{FichaId}");
        }
    }

    private void NoFicha()
    {
        NavigationManager.NavigateTo("/fichas");
    }

    private async Task DeleteAsync(Familia familia)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"¿Estás seguro de que quieres eliminar a {familia.Persona?.NombreCompleto}?" }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;

        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/familias/{familia.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Familiar eliminado correctamente.", Severity.Success);

        // Recargar la lista después de mostrar el mensaje
        await LoadAsync();
    }

    private void NavigateBackToFicha()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}