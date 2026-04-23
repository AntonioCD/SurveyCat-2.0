using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Personas
{
    public partial class PersonaCreate
    {
        private Persona persona = new();

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        //[Parameter] public int BarrioComarcaId { get; set; }

        private async Task CreateAsync()
        {
            //persona.ComarcaId = BarrioComarcaId;
            var responseHttp = await Repository.PostAsync("/api/personas", persona);
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
            NavigationManager.NavigateTo($"/personas");
        }
    }
}