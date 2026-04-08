using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.BarriosComarcas;

public partial class BarrioComarcaCreate
{
    private BarrioComarca barrioComarca = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int MunicipioId { get; set; }

    private async Task CreateAsync()
    {
        barrioComarca.MunicipioId = MunicipioId;
        var responseHttp = await Repository.PostAsync("/api/barriosComarcas", barrioComarca);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/municipios/details/{MunicipioId}");
    }
}