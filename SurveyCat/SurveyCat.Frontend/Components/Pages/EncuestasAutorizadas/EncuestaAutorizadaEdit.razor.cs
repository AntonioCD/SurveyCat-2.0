using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.EncuestasAutorizadas;

public partial class EncuestaAutorizadaEdit
{
    private EncuestaAutorizada? encuestaAutorizada;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public long Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<EncuestaAutorizada>($"api/encuestasAutorizadas/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("encuestasAutorizadas");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            encuestaAutorizada = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        encuestaAutorizada!.Municipio = null;
        encuestaAutorizada.BarrioComarca = null;
        encuestaAutorizada.Caserio = null;
        var responseHttp = await Repository.PutAsync("api/encuestasAutorizadas", encuestaAutorizada);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro guardado.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo("encuestasAutorizadas");
    }
}