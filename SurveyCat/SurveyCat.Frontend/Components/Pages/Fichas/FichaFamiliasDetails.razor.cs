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

    // CORRECCIÓN CLAVE: Inicializada de inmediato para evitar errores de renderizado (ArgumentNullException)
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

        // 1. Cargamos la Ficha si no existe
        if (ficha is null)
        {
            var ok = await LoadFichaAsync();
            if (!ok)
            {
                NoFicha();
                loading = false;
                return;
            }
        }

        // 2. Traemos la lista completa del backend (para el drag-and-drop es óptimo manejar el set completo de la ficha)
        var urlList = $"{baseUrl}/paginated?id={FichaId}&page=1&recordsnumber=100"; // Ajusta el recordsnumber según tus necesidades
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
            // Forzamos el ordenamiento por Item de manera ascendente al mapearlo en la UI
            familias = responseHttp.Response.OrderBy(f => f.Item).ToList();
            totalRecords = familias.Count;
        }

        loading = false;
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
        // Si no hay item o se soltó en un lugar indefinido, abortamos
        if (dropInfo.Item == null || string.IsNullOrEmpty(dropInfo.DropzoneIdentifier)) return;

        // Convertimos el identificador de la zona de destino a número entero
        if (!int.TryParse(dropInfo.DropzoneIdentifier, out int itemDestino)) return;

        var itemMovido = dropInfo.Item;

        // Si lo soltó en la misma posición en la que ya estaba, no gastamos recursos ni llamamos al API
        if (itemMovido.Item == itemDestino) return;

        // 1. Extraemos todos los demás elementos ordenados excluyendo al que se está moviendo
        var listaModificada = familias.Where(f => f.Id != itemMovido.Id).OrderBy(f => f.Item).ToList();

        // 2. Calculamos el índice exacto de inserción en la lista basado en el número de Item destino
        // Restamos 1 porque los Items van de 1 a N, pero los índices de la lista van de 0 a N-1
        int nuevoIndice = itemDestino - 1;

        if (nuevoIndice < 0) nuevoIndice = 0;
        if (nuevoIndice > listaModificada.Count) nuevoIndice = listaModificada.Count;

        // 3. Insertamos el elemento en su lugar exacto
        listaModificada.Insert(nuevoIndice, itemMovido);

        // 4. Regeneramos los consecutivos (1, 2, 3...) de los objetos clonados para romper referencias
        var listaParaEnviar = listaModificada.Select((f, index) => new Familia
        {
            Id = f.Id,
            FichaId = f.FichaId,
            Item = index + 1, // Asigna la secuencia limpia corregida
            PersonaId = f.PersonaId,
            Persona = f.Persona,
            ParentescoId = f.ParentescoId,
            Parentesco = f.Parentesco
        }).ToList();

        // 5. Refrescamos la UI localmente
        familias = listaParaEnviar;
        StateHasChanged();

        // 6. Persistimos los cambios en la base de datos a través de tu arquitectura de capas
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

        // Al recargar mediante LoadAsync(), se ejecutan el conteo y la re-indexación automática del Backend
        await LoadAsync();
        Snackbar.Add("Familiar eliminado correctamente.", Severity.Success);
    }

    private void NavigateBackToFicha()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}