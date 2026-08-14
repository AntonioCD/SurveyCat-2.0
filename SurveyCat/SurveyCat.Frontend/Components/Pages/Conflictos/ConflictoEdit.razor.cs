using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Conflictos;

public partial class ConflictoEdit
{
    private Conflicto? conflicto;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int Id { get; set; }
    [Parameter] public int FichaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Conflicto>($"api/conflictos/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=4");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            conflicto = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        if (conflicto == null) return;

        // Limpiar propiedades de navegación antes de enviar (igual que en OcupanteEdit)
        conflicto.Ficha = null;
        conflicto.TipoConflicto = null;
        conflicto.ViaGestion = null;

        var responseHttp = await Repository.PutAsync<Conflicto>("/api/conflictos", conflicto);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Snackbar.Add("Conflicto actualizado exitosamente.", Severity.Success);
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=4");
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=4");
    }
}