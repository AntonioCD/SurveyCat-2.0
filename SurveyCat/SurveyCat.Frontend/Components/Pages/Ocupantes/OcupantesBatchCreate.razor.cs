using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Ocupantes;

public partial class OcupantesBatchCreate
{
    // ViewModel local para gestionar la fila en edición
    public class OcupanteItemVM
    {
        public Persona Persona { get; set; } = null!;
        public Diccionario? TipoOcupante { get; set; }
        public Diccionario? Parentesco { get; set; }
    }

    private List<OcupanteItemVM> listaOcupantesVM = new();
    private List<Diccionario> listaTipoOcupante = new();
    private List<Diccionario> listaParentesco = new();

    private bool loading = true;
    private bool guardando = false;
    private long? _tipoFamiliarId;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Parameter] public int FichaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDiccionariosAsync();
    }

    private async Task LoadDiccionariosAsync()
    {
        loading = true;
        var responseHttp = await Repository.GetAsync<List<Diccionario>>("/api/diccionarios/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            loading = false;
            return;
        }

        var diccionarios = responseHttp.Response;
        if (diccionarios != null)
        {
            listaParentesco = diccionarios.Where(x => x.Catalogo == Catalogos.Parentesco).ToList();
            listaTipoOcupante = diccionarios.Where(x => x.Catalogo == Catalogos.TipoOcupante).ToList();

            var familiar = listaTipoOcupante.FirstOrDefault(x => x.Nombre.Equals("Familiar", StringComparison.OrdinalIgnoreCase));
            if (familiar != null)
            {
                _tipoFamiliarId = familiar.Id;
            }
        }

        loading = false;
    }

    private async Task BuscarYAgregarPersonaAsync()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };

        var parameters = new DialogParameters<PersonaSearch>
        {
            { x => x.SoloNaturales, true }
        };

        var dialog = await DialogService.ShowAsync<PersonaSearch>("Buscar Persona", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is Persona personaSeleccionada)
        {
            // Validar que la persona no esté ya en la lista local
            if (listaOcupantesVM.Any(x => x.Persona.Id == personaSeleccionada.Id))
            {
                Snackbar.Add("Esta persona ya se encuentra agregada en la lista.", Severity.Warning);
                return;
            }

            // Obtener el detalle completo de la persona
            var personaCompleta = await GetPersonaDetailsAsync(personaSeleccionada.Id);
            if (personaCompleta != null)
            {
                listaOcupantesVM.Add(new OcupanteItemVM
                {
                    Persona = personaCompleta
                });
                Snackbar.Add($"Persona '{personaCompleta.NombreCompleto}' agregada.", Severity.Info);
            }
        }
    }

    private async Task<Persona?> GetPersonaDetailsAsync(long personaId)
    {
        var responseHttp = await Repository.GetAsync<Persona>($"api/personas/{personaId}");
        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return null;
        }
        return responseHttp.Response;
    }

    private void OnTipoOcupanteChanged(OcupanteItemVM item, Diccionario? nuevoTipo)
    {
        item.TipoOcupante = nuevoTipo;

        // Si cambia a algo diferente de "Familiar", se limpia el parentesco
        if (nuevoTipo?.Id != _tipoFamiliarId)
        {
            item.Parentesco = null;
        }
    }

    private bool EsFamiliar(OcupanteItemVM item)
    {
        return item.TipoOcupante != null && item.TipoOcupante.Id == _tipoFamiliarId;
    }

    private void RemoverOcupante(OcupanteItemVM item)
    {
        listaOcupantesVM.Remove(item);
    }

    private async Task GuardarTodosAsync()
    {
        // 1. Validaciones previas
        foreach (var item in listaOcupantesVM)
        {
            if (item.TipoOcupante == null)
            {
                Snackbar.Add($"Debe seleccionar el Tipo de Ocupante para: {item.Persona.NombreCompleto}", Severity.Warning);
                return;
            }

            if (EsFamiliar(item) && item.Parentesco == null)
            {
                Snackbar.Add($"Debe seleccionar el Parentesco para el familiar: {item.Persona.NombreCompleto}", Severity.Warning);
                return;
            }
        }

        guardando = true;
        int guardadosConExito = 0;

        // 2. Procesamiento secuencial (o bien adaptado a tu endpoint de backend)
        foreach (var item in listaOcupantesVM)
        {
            var nuevoOcupante = new Ocupante
            {
                FichaId = FichaId,
                PersonaId = item.Persona.Id,
                TipoOcupanteId = item.TipoOcupante!.Id,
                ParentescoId = item.Parentesco?.Id
            };

            var responseHttp = await Repository.PostAsync<Ocupante>("/api/ocupantes", nuevoOcupante);
            if (!responseHttp.Error)
            {
                guardadosConExito++;
            }
            else
            {
                var error = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add($"Error al guardar a {item.Persona.NombreCompleto}: {error}", Severity.Error);
            }
        }

        guardando = false;

        if (guardadosConExito > 0)
        {
            Snackbar.Add($"Se registraron {guardadosConExito} ocupante(s) con éxito.", Severity.Success);
            Return();
        }
    }

    private void Return()
    {
        NavigationManager.NavigateTo($"/fichas/edit/{FichaId}?tab=2");
    }
}