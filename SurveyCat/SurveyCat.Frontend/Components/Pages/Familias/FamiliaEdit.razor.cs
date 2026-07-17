using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Familias;

public partial class FamiliaEdit
{
    private Familia? familia;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int Id { get; set; }
    [Parameter] public int FichaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Familia>($"api/familias/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            familia = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        familia!.Ficha = null;
        familia.Persona = null;
        familia.Parentesco = null;

        var responseHttp = await Repository.PutAsync<Familia>("api/familias", familia);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        // Volver a la ficha con la pestaña de Núcleo Familiar activa (tab=2)
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
        Snackbar.Add("Familiar guardado exitosamente.", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}