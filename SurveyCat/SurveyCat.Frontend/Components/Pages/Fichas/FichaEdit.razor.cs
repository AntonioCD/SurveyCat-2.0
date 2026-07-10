using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Fichas
{
    public partial class FichaEdit
    {
        private Ficha? ficha;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var responseHttp = await Repository.GetAsync<Ficha>($"api/fichas/{Id}");

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("fichas");
                }
                else
                {
                    var messageError = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messageError!, Severity.Error);
                }
            }
            else
            {
                ficha = responseHttp.Response;
            }
        }

        private async Task EditAsync()
        {
            ficha!.Municipio = null;
            ficha.Sector = null;
            ficha.Encuestador = null;
            ficha.TecnicoCatastral = null;
            ficha.Coordinador = null;
            ficha.BarrioComarca = null;
            ficha.Caserio = null;
            ficha.UnidadMedida = null;
            ficha.OrigenTierra = null;
            ficha.ServidumbreAgua = null;
            ficha.ServidumbrePase = null;
            ficha.ServidumbreOtra = null;
            ficha.Estado = null;
            ficha.Informante = null;
            ficha.RelacionInformanteParcela = null;
            ficha.RelacionInformantePropietario = null;
            ficha.Propietarios = null;

            var responseHttp = await Repository.PutAsync("api/fichas", ficha);

            if (responseHttp.Error)
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
                return;
            }

            Snackbar.Add("Ficha guardada exitosamente.", Severity.Success);

            // Recargar la misma página con la pestaña de propietarios activa
            NavigationManager.NavigateTo($"/fichas/edit/{Id}?tab=1", forceLoad: true);
        }

        private void Return()
        {
            NavigationManager.NavigateTo($"/fichas");
        }
    }
}