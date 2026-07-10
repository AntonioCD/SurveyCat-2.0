using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Propietarios;

public partial class PropietarioCreate
{
    private Propietario propietario = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }

    private async Task CreateAsync()
    {
        propietario.FichaId = FichaId;
        var responseHttp = await Repository.PostAsync("/api/propietarios", propietario);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    // En PropietarioCreate.razor.cs y PropietarioEdit.razor.cs
    private void Return()
    {
        // Si viene de la ficha, vuelve a la ficha
        if (NavigationManager.Uri.Contains("/fichas/edit/"))
        {
            NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=1");
        }
        else
        {
            // Si viene de la vista independiente, va a la lista de propietarios
            NavigationManager.NavigateTo($"/fichas/propietarios/details/{FichaId}");
        }
    }
}