using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaForm
{
    private EditContext editContext = null!;
    private bool loading = true;
    private bool isInitialized = false;
    private List<Diccionario>? diccionarios;
    private List<Diccionario> listaUnidadMedida = new();
    private List<Diccionario> listaEstado = new();
    private List<Diccionario> listaOrigenTierra = new();
    private List<Diccionario> listaRelacionInformanteParcela = new();
    private List<Diccionario> listaRelacionInformantePropietario = new();
    private List<Diccionario> listaServidumbre = new();
    private List<Departamento>? departamentos;
    private List<Municipio>? municipios;
    private List<Sector>? sectores;
    private List<BarrioComarca>? barriosComarcas;
    private List<Caserio>? caserios;
    private List<PersonalEncuesta>? personalEncuestas;
    private List<PersonalEncuesta>? listaEncuestadores;
    private List<PersonalEncuesta>? listaTecnicosCatastrales;
    private List<PersonalEncuesta>? listaSupervisores;

    private Persona? informante = new();

    private Diccionario? selectedUnidadMedida;
    private Diccionario? selectedEstado;
    private Diccionario? selectedOrigenTierra;
    private Diccionario? selectedRelacionInformanteParcela;
    private Diccionario? selectedRelacionInformantePropietario;
    private Diccionario? selectedServidumbreAgua;
    private Diccionario? selectedServidumbrePase;
    private Diccionario? selectedServidumbreOtra;
    private Departamento? selectedDepartamento;
    private Municipio? selectedMunicipio;
    private Sector? selectedSector;
    private BarrioComarca? selectedBarrioComarca;
    private Caserio? selectedCaserio;
    private PersonalEncuesta? selectedEncuestador;
    private PersonalEncuesta? selectedTecnicoCatastral;
    private PersonalEncuesta? selectedSupervisor;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [EditorRequired, Parameter] public Ficha Ficha { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnParametersSet()
    {
        if (isInitialized)
            return;

        var uri = new Uri(NavigationManager.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (int.TryParse(query.Get("tab"), out int tabIndex))
        {
            activeTabIndex = tabIndex;
        }

        if (Ficha == null)
        {
            Ficha = new Ficha();
        }

        if (editContext == null || editContext.Model != Ficha)
        {
            editContext = new EditContext(Ficha);
        }

        // Disparar carga asíncrona sin bloquear el render
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        loading = true;

        try
        {
            // Cargar en paralelo las llamadas independientes
            var tareaDiccionarios = LoadDiccionariosAsync();
            var tareaPersonal = LoadPersonalEncuestaAsync();
            var tareaDepartamentos = LoadDepartamentosAsync();

            await Task.WhenAll(tareaDiccionarios, tareaPersonal, tareaDepartamentos);

            if (Ficha.Id != 0)
            {
                // Cargar todos los datos relacionados para edición
                await LoadRelatedDataForEditAsync();

                // Asignar valores seleccionados
                selectedEncuestador = Ficha.Encuestador;
                selectedTecnicoCatastral = Ficha.TecnicoCatastral;
                selectedSupervisor = Ficha.Coordinador;
                selectedUnidadMedida = Ficha.UnidadMedida;
                selectedOrigenTierra = Ficha.OrigenTierra;
                selectedServidumbreAgua = Ficha.ServidumbreAgua;
                selectedServidumbrePase = Ficha.ServidumbrePase;
                selectedServidumbreOtra = Ficha.ServidumbreOtra;
                selectedEstado = Ficha.Estado;
                selectedRelacionInformanteParcela = Ficha.RelacionInformanteParcela;
                selectedRelacionInformantePropietario = Ficha.RelacionInformantePropietario;

                if (Ficha.InformanteId > 0 && (informante == null || informante.Id != Ficha.InformanteId))
                {
                    informante = await GetPersonaDetails(Ficha.InformanteId);
                }

                // Extraer consecutivo del código de encuesta
                if (!string.IsNullOrWhiteSpace(Ficha.CodEncuesta) && Ficha.CodEncuesta.Length >= 4)
                {
                    Ficha.Consecutivo = Ficha.CodEncuesta.Substring(Ficha.CodEncuesta.Length - 4);
                }
            }
            else
            {
                // Para nuevo registro, establecer estado inicial
                Ficha.EstadoId = listaEstado
                    .Where(e => e.Nombre.Contains("Digitado"))
                    .Select(e => e.Id)
                    .FirstOrDefault();
            }
        }
        finally
        {
            loading = false;
            isInitialized = true;
            StateHasChanged();
        }
    }

    private async Task LoadRelatedDataForEditAsync()
    {
        try
        {
            // 1. Cargar el Municipio completo
            if (Ficha.MunicipioId > 0)
            {
                var municipioResponse = await Repository.GetAsync<Municipio>($"/api/municipios/{Ficha.MunicipioId}");
                if (!municipioResponse.Error && municipioResponse.Response != null)
                {
                    var municipio = municipioResponse.Response;

                    // 2. Cargar el Departamento del Municipio
                    if (municipio.DepartamentoId > 0)
                    {
                        var deptoResponse = await Repository.GetAsync<Departamento>($"/api/departamentos/{municipio.DepartamentoId}");
                        if (!deptoResponse.Error && deptoResponse.Response != null)
                        {
                            selectedDepartamento = deptoResponse.Response;

                            // 3. Cargar los municipios del departamento
                            await LoadMunicipiosAsync(selectedDepartamento.Id);

                            // 4. Seleccionar el municipio correcto
                            selectedMunicipio = municipios?.FirstOrDefault(m => m.Id == Ficha.MunicipioId);

                            if (selectedMunicipio != null)
                            {
                                // 5. Cargar Sectores del municipio
                                await LoadSectoresAsync(selectedMunicipio.Id);

                                // 6. Seleccionar el sector correcto
                                if (Ficha.SectorId > 0)
                                {
                                    selectedSector = sectores?.FirstOrDefault(s => s.Id == Ficha.SectorId);
                                }

                                // 7. Cargar Barrios/Comarcas del municipio
                                await LoadBarriosComarcasAsync(selectedMunicipio.Id);

                                // 8. Seleccionar el Barrio/Comarca correcto
                                if (Ficha.BarrioComarcaId.HasValue && Ficha.BarrioComarcaId.Value > 0)
                                {
                                    selectedBarrioComarca = barriosComarcas?.FirstOrDefault(b => b.Id == Ficha.BarrioComarcaId.Value);

                                    if (selectedBarrioComarca != null)
                                    {
                                        // 9. Cargar Caserios del Barrio/Comarca
                                        await LoadCaseriosAsync(selectedBarrioComarca.Id);

                                        // 10. Seleccionar el Caserio correcto
                                        if (Ficha.CaserioId.HasValue && Ficha.CaserioId.Value > 0)
                                        {
                                            selectedCaserio = caserios?.FirstOrDefault(c => c.Id == Ficha.CaserioId.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al cargar datos relacionados: {ex.Message}", Severity.Error);
        }
    }

    private async Task LoadPersonalEncuestaAsync()
    {
        var responseHttp = await Repository.GetAsync<List<PersonalEncuesta>>("/api/personalEncuestas/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        personalEncuestas = responseHttp.Response;

        if (personalEncuestas != null)
        {
            listaEncuestadores = personalEncuestas.Where(x => x.TipoRol == TipoRol.Encuestador).ToList();
            listaTecnicosCatastrales = personalEncuestas.Where(x => x.TipoRol == TipoRol.TécnicoCatastral).ToList();
            listaSupervisores = personalEncuestas.Where(x => x.TipoRol == TipoRol.Supervisor).ToList();
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
            listaEstado = diccionarios.Where(x => x.Catalogo == Catalogos.EstadoEncuesta).ToList();
            listaOrigenTierra = diccionarios.Where(x => x.Catalogo == Catalogos.OrigenTierra).ToList();
            listaRelacionInformanteParcela = diccionarios.Where(x => x.Catalogo == Catalogos.RelacionInformanteParcela).ToList();
            listaRelacionInformantePropietario = diccionarios.Where(x => x.Catalogo == Catalogos.RelacionInformantePropietario).ToList();
            listaServidumbre = diccionarios.Where(x => x.Catalogo == Catalogos.Servidumbre).ToList();
        }
    }

    private async Task LoadDepartamentosAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Departamento>>("/api/departamentos/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        departamentos = responseHttp.Response;
    }

    private async Task LoadMunicipiosAsync(int departamentoId)
    {
        var responseHttp = await Repository.GetAsync<List<Municipio>>($"/api/municipios/combo/{departamentoId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        municipios = responseHttp.Response;
    }

    private async Task LoadSectoresAsync(int municipioId)
    {
        var responseHttp = await Repository.GetAsync<List<Sector>>($"/api/sectores/combo/{municipioId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        sectores = responseHttp.Response;
    }

    private async Task LoadBarriosComarcasAsync(int municipioId)
    {
        var responseHttp = await Repository.GetAsync<List<BarrioComarca>>($"/api/barriosComarcas/combo/{municipioId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        barriosComarcas = responseHttp.Response;
    }

    private async Task LoadCaseriosAsync(int comarcaId)
    {
        var responseHttp = await Repository.GetAsync<List<Caserio>>($"/api/caserios/combo/{comarcaId}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        caserios = responseHttp.Response;
    }

    private void EncuestadorChanged(PersonalEncuesta encuestador)
    {
        if (encuestador == null) return;
        selectedEncuestador = encuestador;
        Ficha.EncuestadorId = encuestador.Id;
    }

    private void TecnicoCatastralChanged(PersonalEncuesta tecnicoCatastral)
    {
        if (tecnicoCatastral == null) return;
        selectedTecnicoCatastral = tecnicoCatastral;
        Ficha.TecnicoCatastralId = tecnicoCatastral.Id;
        GenerarCodigoEncuesta();
    }

    private void SupervisorChanged(PersonalEncuesta supervisor)
    {
        if (supervisor == null) return;
        selectedSupervisor = supervisor;
        Ficha.CoordinadorId = supervisor.Id;
    }

    private void UnidadMedidaChanged(Diccionario unidadMedida)
    {
        if (unidadMedida == null) return;
        selectedUnidadMedida = unidadMedida;
        Ficha.UnidadMedidaId = unidadMedida.Id;
    }

    private void EstadoChanged(Diccionario estado)
    {
        if (estado == null) return;
        selectedEstado = estado;
        Ficha.EstadoId = estado.Id;
    }

    private void OrigenTierraChanged(Diccionario origenTierra)
    {
        if (origenTierra == null) return;
        selectedOrigenTierra = origenTierra;
        Ficha.OrigenTierraId = origenTierra.Id;
    }

    private void RelacionInformanteParcelaChanged(Diccionario relacionInformanteParcela)
    {
        if (relacionInformanteParcela == null) return;
        selectedRelacionInformanteParcela = relacionInformanteParcela;
        Ficha.RelacionInformanteParcelaId = relacionInformanteParcela.Id;
    }

    private void RelacionInformantePropietarioChanged(Diccionario relacionInformantePropietario)
    {
        if (relacionInformantePropietario == null) return;
        selectedRelacionInformantePropietario = relacionInformantePropietario;
        Ficha.RelacionInformantePropietarioId = relacionInformantePropietario.Id;
    }

    private void ServidumbreAguaChanged(Diccionario servidumbreAgua)
    {
        if (servidumbreAgua == null) return;
        selectedServidumbreAgua = servidumbreAgua;
        Ficha.ServidumbreAguaId = servidumbreAgua.Id;
    }

    private void ServidumbrePaseChanged(Diccionario servidumbrePase)
    {
        if (servidumbrePase == null) return;
        selectedServidumbrePase = servidumbrePase;
        Ficha.ServidumbrePaseId = servidumbrePase.Id;
    }

    private void ServidumbreOtraChanged(Diccionario servidumbreOtra)
    {
        if (servidumbreOtra == null) return;
        selectedServidumbreOtra = servidumbreOtra;
        Ficha.ServidumbreOtraId = servidumbreOtra.Id;
    }

    private async Task DepartamentoChangedAsync(Departamento departamento)
    {
        if (departamento == null)
            return;

        selectedDepartamento = departamento;
        selectedMunicipio = null;
        selectedSector = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        municipios = null;
        sectores = null;
        barriosComarcas = null;
        caserios = null;

        // Limpiar los IDs
        Ficha.MunicipioId = 0;
        Ficha.SectorId = 0;
        Ficha.BarrioComarcaId = null;
        Ficha.CaserioId = null;

        await LoadMunicipiosAsync(departamento.Id);
    }

    private async Task MunicipioChangedAsync(Municipio municipio)
    {
        if (municipio == null)
            return;

        selectedMunicipio = municipio;
        Ficha.MunicipioId = municipio.Id;
        selectedSector = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        sectores = null;
        barriosComarcas = null;
        caserios = null;

        // Limpiar los IDs
        Ficha.SectorId = 0;
        Ficha.BarrioComarcaId = null;
        Ficha.CaserioId = null;

        await LoadSectoresAsync(municipio.Id);
        await LoadBarriosComarcasAsync(municipio.Id);
        GenerarCodigoEncuesta();
    }

    private void SectorChanged(Sector sector)
    {
        if (sector == null)
            return;

        selectedSector = sector;
        Ficha.SectorId = sector.Id;
        GenerarCodigoEncuesta();
    }

    private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    {
        if (barrioComarca == null)
            return;

        selectedBarrioComarca = barrioComarca;
        Ficha.BarrioComarcaId = barrioComarca.Id;
        selectedCaserio = null;
        caserios = null;

        // Limpiar el ID
        Ficha.CaserioId = null;

        await LoadCaseriosAsync(barrioComarca.Id);
    }

    private void CaserioChanged(Caserio caserio)
    {
        if (caserio == null)
            return;

        selectedCaserio = caserio;
        Ficha.CaserioId = caserio.Id;
    }

    private async Task<IEnumerable<PersonalEncuesta>> SearchEncuestador(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaEncuestadores!;

        return listaEncuestadores!
            .Where(c => c.Persona!.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<PersonalEncuesta>> SearchTecnicoCatastral(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaTecnicosCatastrales!;

        return listaTecnicosCatastrales!
            .Where(c => c.Persona!.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<PersonalEncuesta>> SearchSupervisor(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaSupervisores!;

        return listaSupervisores!
            .Where(c => c.Persona!.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchUnidadMedida(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaUnidadMedida!;

        return listaUnidadMedida!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchEstado(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaEstado!;

        return listaEstado!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchOrigenTierra(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaOrigenTierra!;

        return listaOrigenTierra!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchRelacionInformanteParcela(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaRelacionInformanteParcela!;

        return listaRelacionInformanteParcela!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchRelacionInformantePropietario(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaRelacionInformantePropietario!;

        return listaRelacionInformantePropietario!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbreAgua(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaServidumbre!;

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbrePase(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaServidumbre!;

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbreOtra(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return listaServidumbre!;

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Departamento>> SearchDepartamento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return departamentos!;

        return departamentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Municipio>> SearchMunicipio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return municipios!;

        return municipios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Sector>> SearchSector(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return sectores!;

        return sectores!
            .Where(c => c.NumeroSector.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<BarrioComarca>> SearchBarrioComarca(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return barriosComarcas!;

        return barriosComarcas!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Caserio>> SearchCaserio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
            return caserios!;

        return caserios!
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

        if (!result.Canceled && result.Data is Persona personaSeleccionada)
        {
            var informanteResult = await GetPersonaDetails(personaSeleccionada.Id);

            if (informanteResult != null)
            {
                informante = informanteResult;
                Ficha.InformanteId = informante.Id;
                Snackbar.Add("Datos del entrevistado cargados con éxito.", Severity.Success);
            }
            else
            {
                Snackbar.Add("No se pudieron cargar los datos del entrevistado.", Severity.Warning);
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
            return null;
        }

        if (responseHttp.Response == null)
        {
            Snackbar.Add("No se encontraron los detalles de la persona.", Severity.Warning);
            return null;
        }

        return responseHttp.Response;
    }

    private void GenerarCodigoEncuesta()
    {
        if (selectedTecnicoCatastral!.Id != 0 &&
            selectedMunicipio!.Id != 0 &&
            selectedSector!.Id != 0 &&
            (!string.IsNullOrWhiteSpace(Ficha.Consecutivo) && Ficha.Consecutivo.Length == 4))
        {
            string inicial = selectedSector?.NumeroSector?.Substring(0, 1).ToUpper() ?? "";

            string sector = inicial switch
            {
                "R" => "RUR",
                "U" => "URB",
                _ => "000"
            };
            string codMuni = selectedMunicipio.CodMuni;
            string codTecnico = selectedTecnicoCatastral.Codigo;
            string consecutivo = Ficha.Consecutivo;

            Ficha.CodEncuesta = $"{sector}{codMuni}{codTecnico}{codTecnico}{consecutivo}";
        }
        else
        {
            Ficha.CodEncuesta = string.Empty;
        }
    }
}