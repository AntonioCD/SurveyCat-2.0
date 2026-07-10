using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaCreate
{
    private Ficha ficha = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync<Ficha>("/api/fichas", ficha);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        // Obtener el ID de la ficha creada - Cast a Ficha
        var fichaCreada = responseHttp.Response as Ficha;

        if (fichaCreada == null)
        {
            Snackbar.Add("Error al obtener los datos de la ficha creada.", Severity.Error);
            return;
        }

        Snackbar.Add($"¡Ficha {fichaCreada.CodEncuesta} creada exitosamente! Ahora puedes agregar propietarios.", Severity.Success);

        // Redirigir a la edición con la pestaña de propietarios activa
        NavigationManager.NavigateTo($"/fichas/edit/{fichaCreada.Id}?tab=1");
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas");
    }
}