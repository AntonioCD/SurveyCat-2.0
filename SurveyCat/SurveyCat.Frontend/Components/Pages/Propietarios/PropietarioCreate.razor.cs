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
    [Parameter] public EventCallback OnPropietarioChanged { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

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

        Snackbar.Add("Registro creado con éxito.", Severity.Success);

        // Notificar al padre y dejar que él maneje el cierre y refresco
        await OnPropietarioChanged.InvokeAsync();
        await OnCancel.InvokeAsync();
    }

    private async Task Return()
    {
        await OnCancel.InvokeAsync();
    }
}