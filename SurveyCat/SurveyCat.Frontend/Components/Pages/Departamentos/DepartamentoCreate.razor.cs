using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;

namespace SurveyCat.Frontend.Components.Pages.Departamentos
{
    public partial class DepartamentoCreate
    {
        private Departamento departamento = new();

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/departamentos", departamento);
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
            NavigationManager.NavigateTo("/departamentos");
        }
    }
}