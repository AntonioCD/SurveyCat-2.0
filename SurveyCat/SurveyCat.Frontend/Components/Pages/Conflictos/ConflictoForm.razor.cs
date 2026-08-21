using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Conflictos;

public partial class ConflictoForm
{
    private EditContext editContext = null!;
    private bool loading = true;
    private bool isInitialized = false;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaConflictos = new();
    private List<Diccionario> listaViasGestion = new();

    private Diccionario? selectedConflicto;
    private Diccionario? selectedViaGestion;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public Conflicto Conflicto { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnParametersSet()
    {
        if (isInitialized)
            return;

        if (Conflicto == null)
        {
            Conflicto = new Conflicto();
        }

        if (editContext == null || editContext.Model != Conflicto)
        {
            editContext = new EditContext(Conflicto);
        }

        // Disparar carga asíncrona sin bloquear el render
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        loading = true;

        try
        {
            await LoadDiccionariosAsync();

            if (Conflicto.Id != 0)
            {
                selectedConflicto = listaConflictos
                    .FirstOrDefault(x => x.Id == Conflicto.TipoConflictoId);

                selectedViaGestion = listaViasGestion
                    .FirstOrDefault(x => x.Id == Conflicto.ViaGestionId);
            }
            else
            {
                // Limpiar selección para registros nuevos
                selectedConflicto = null;
                selectedViaGestion = null;
            }
        }
        finally
        {
            loading = false;
            isInitialized = true;
            StateHasChanged();
        }
    }

    private async Task LoadDiccionariosAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Diccionario>>("/api/diccionarios/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        diccionarios = responseHttp.Response;

        if (diccionarios != null)
        {
            listaConflictos = diccionarios
                .Where(x => x.Catalogo == Catalogos.ClaseConflicto)
                .ToList();

            listaViasGestion = diccionarios
                .Where(x => x.Catalogo == Catalogos.GestionConflicto)
                .ToList();
        }
    }

    private void ConflictoChanged(Diccionario? conflicto)
    {
        selectedConflicto = conflicto;
        Conflicto.TipoConflictoId = conflicto?.Id ?? 0;
    }

    private void ViaGestionChanged(Diccionario? viaGestion)
    {
        selectedViaGestion = viaGestion;
        Conflicto.ViaGestionId = viaGestion?.Id ?? 0;
    }

    private async Task<IEnumerable<Diccionario>> SearchConflicto(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaConflictos!;

        return listaConflictos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchViaGestion(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaViasGestion!;

        return listaViasGestion!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
}