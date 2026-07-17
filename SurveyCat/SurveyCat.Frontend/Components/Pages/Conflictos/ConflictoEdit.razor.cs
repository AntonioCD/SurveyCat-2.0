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
    [Inject] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public int Id { get; set; }
    [Parameter] public int FichaId { get; set; }
    [Parameter] public bool IsEmbedded { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Conflicto>($"api/conflictos/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                if (IsEmbedded)
                {
                    MudDialog.Cancel();
                }
                else
                {
                    NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
                }
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
                if (IsEmbedded)
                {
                    MudDialog.Cancel();
                }
            }
        }
        else
        {
            conflicto = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        conflicto!.Ficha = null;
        conflicto.TipoConflicto = null;
        conflicto.ViaGestion = null;

        var responseHttp = await Repository.PutAsync<Conflicto>("api/conflictos", conflicto);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Snackbar.Add("Conflicto guardado exitosamente.", Severity.Success);

        if (IsEmbedded)
        {
            // Si viene de la ficha, cerrar el diálogo y recargar
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            // Si es página independiente, navegar
            NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
        }
    }

    private void Return()
    {
        if (IsEmbedded)
        {
            MudDialog.Cancel();
        }
        else
        {
            NavigationManager.NavigateTo($"/fichas/conflictos/details/{FichaId}");
        }
    }
}