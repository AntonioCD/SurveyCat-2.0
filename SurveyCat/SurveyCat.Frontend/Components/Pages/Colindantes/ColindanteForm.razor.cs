using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Colindantes;

public partial class ColindanteForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaPuntosCardinales = new();
    private List<Diccionario> listaConflictos = new();
    private List<Diccionario> listaViasGestion = new();
    private Persona? persona = new();

    private Diccionario? selectedPuntoCardinal = new();
    private Diccionario? selectedConflicto = new();
    private Diccionario? selectedViaGestion = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public Colindante Colindante { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Colindante == null)
        {
            Colindante = new Colindante();
        }

        // Recrear el EditContext si el modelo cambió
        if (editContext == null || editContext.Model != Colindante)
        {
            editContext = new EditContext(Colindante);
        }

        await LoadDiccionariosAsync();

        if (Colindante.Id != 0)
        {
            selectedPuntoCardinal = listaPuntosCardinales.Where(x => x.Id == Colindante.PuntoCardinalId).FirstOrDefault();
            selectedConflicto = listaConflictos.Where(x => x.Id == Colindante.ConflictoId).FirstOrDefault();
            selectedViaGestion = listaViasGestion.Where(x => x.Id == Colindante.ViaGestionId).FirstOrDefault();

            if (persona == null || persona.Id != Colindante.PersonaId)
            {
                persona = await GetPersonaDetails(Colindante.PersonaId);
            }
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
            listaPuntosCardinales = diccionarios.Where(x => x.Catalogo == Catalogos.PuntoCardinal).ToList();
            listaConflictos = diccionarios.Where(x => x.Catalogo == Catalogos.ClaseConflicto).ToList();
            listaViasGestion = diccionarios.Where(x => x.Catalogo == Catalogos.GestionConflicto).ToList();
        }
    }

    private void PuntoCardinalChanged(Diccionario puntoCardinal)
    {
        if (puntoCardinal != null)
        {
            selectedPuntoCardinal = puntoCardinal;
            Colindante.PuntoCardinalId = puntoCardinal!.Id;
        }
    }

    private void ConflictoChanged(Diccionario conflicto)
    {
        if (conflicto != null)
        {
            selectedConflicto = conflicto;
            Colindante.ConflictoId = conflicto!.Id;
        }
    }

    private void ViaGestionChanged(Diccionario viaGestion)
    {
        if (viaGestion != null)
        {
            selectedViaGestion = viaGestion;
            Colindante.ViaGestionId = viaGestion!.Id;
        }
    }

    private async Task<IEnumerable<Diccionario>> SearchPuntoCardinal(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaPuntosCardinales!;
        }

        return listaPuntosCardinales!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchConflicto(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaConflictos!;
        }

        return listaConflictos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchViaGestion(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaViasGestion!;
        }

        return listaViasGestion!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task ShowModalPersonaSearchAsync()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<PersonaSearch>("Buscar Persona", options);
        var result = await dialog.Result;

        // Verificamos si el usuario seleccionó un registro en el modal
        if (!result.Canceled && result.Data is Persona personaSeleccionada)
        {
            // 1. LLAMADO ASÍNCRONO: Esperamos a que la API traiga los detalles completos
            var personaResult = await GetPersonaDetails(personaSeleccionada.Id);

            // 2. VALIDACIÓN: Evaluamos si devolvió a la persona o si falló (null)
            if (personaResult != null)
            {
                // Asignamos el resultado a la variable que maneja tu formulario
                persona = personaResult;
                Colindante.PersonaId = persona.Id;

                // Si necesitas refrescar cascadas asociadas al informante (municipios, depto, etc.), este es el lugar:
                // await CargarCascadasDelInformanteAsync(informante);

                Snackbar.Add("Datos de la persona cargados con éxito.", Severity.Success);
            }
            else
            {
                // Opcional: Lógica en caso de que no se haya podido recuperar la data completa
                Snackbar.Add("No se pudieron cargar los datos de la persona.", Severity.Warning);
            }

            StateHasChanged();
        }
    }

    private async Task<Persona?> GetPersonaDetails(long personaId)
    {
        var responseHttp = await Repository.GetAsync<Persona>($"api/personas/{personaId}");

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return null; // Retorna null en caso de error
        }

        if (responseHttp.Response == null)
        {
            Snackbar.Add("No se encontraron los detalles de la persona.", Severity.Warning);
            return null; // Retorna null si la API respondió vacío
        }

        return responseHttp.Response; // Retorna la Persona encontrada con éxito
    }
}