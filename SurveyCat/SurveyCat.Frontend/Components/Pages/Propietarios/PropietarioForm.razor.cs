using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Propietarios;

public partial class PropietarioForm
{
    private EditContext editContext = null!;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaUnidadMedida = new();
    private List<Diccionario> listaDocumento = new();
    private Persona? persona = new();

    private Diccionario? selectedUnidadMedida = new();
    private Diccionario? selectedDocumento = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [EditorRequired, Parameter] public Propietario Propietario { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Propietario == null)
        {
            Propietario = new Propietario();
        }

        // Recrear el EditContext si el modelo cambió
        if (editContext == null || editContext.Model != Propietario)
        {
            editContext = new EditContext(Propietario);
        }

        await LoadDiccionariosAsync();

        if (Propietario.Id != 0)
        {
            selectedUnidadMedida = listaUnidadMedida.Where(x => x.Id == Propietario.UnidadMedidaId).FirstOrDefault();
            selectedDocumento = listaDocumento.Where(x => x.Id == Propietario.DocumentoId).FirstOrDefault();

            if (persona == null || persona.Id != Propietario.PersonaId)
            {
                persona = await GetPersonaDetails(Propietario.PersonaId);
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
            listaUnidadMedida = diccionarios.Where(x => x.Catalogo == Catalogos.UnidadMedida).ToList();
            listaDocumento = diccionarios.Where(x => x.Catalogo == Catalogos.Documento).ToList();
        }
    }

    private void UnidadMedidaChanged(Diccionario unidadMedida)
    {
        selectedUnidadMedida = unidadMedida;
        Propietario.UnidadMedidaId = unidadMedida.Id;
    }

    private void DocumentoChanged(Diccionario documento)
    {
        selectedDocumento = documento;
        Propietario.DocumentoId = documento.Id;
    }

    private async Task<IEnumerable<Diccionario>> SearchUnidadMedida(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaUnidadMedida!;
        }

        return listaUnidadMedida!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchDocumento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaDocumento!;
        }

        return listaDocumento!
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
                Propietario.PersonaId = persona.Id;

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