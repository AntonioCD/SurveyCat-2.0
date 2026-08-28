using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Personas;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using System.Security.Claims;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaForm
{
    private EditContext editContext = null!;
    private bool loading = true;
    private bool isInitialized = false;
    private bool esInformantePropietario = false;
    private List<EncuestaAutorizada>? encuestasAutorizadasDisponibles = new();
    private List<Diccionario>? diccionarios = new();
    private List<Diccionario> listaUnidadMedida = new();
    private List<Diccionario> listaEstado = new();
    private List<Diccionario> listaOrigenTierra = new();
    private List<Diccionario> listaRelacionInformanteParcela = new();
    private List<Diccionario> listaRelacionInformantePropietario = new();
    private List<Diccionario> listaServidumbre = new();
    private List<Departamento>? departamentos = new();
    private List<Municipio>? municipios = new();

    //private List<Sector>? sectores;
    private List<BarrioComarca>? barriosComarcas = new();

    private List<Caserio>? caserios;
    private List<PersonalEncuesta>? personalEncuestas = new();
    private List<PersonalEncuesta>? listaEncuestadores = new();
    private List<PersonalEncuesta>? listaTecnicosCatastrales = new();
    private List<PersonalEncuesta>? listaSupervisores = new();

    private Persona? informante = new();

    private EncuestaAutorizada? selectedEncuestaAutorizada;
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

    //private Sector? selectedSector;
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

    protected override async Task OnParametersSetAsync()
    {
        //if (isInitialized)
        //    return;

        //var uri = new Uri(NavigationManager.Uri);
        //var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        //if (int.TryParse(query.Get("tab"), out int tabIndex))
        //{
        //    activeTabIndex = tabIndex;
        //}

        //if (Ficha == null)
        //{
        //    Ficha = new Ficha();
        //}

        //if (editContext == null || editContext.Model != Ficha)
        //{
        //    editContext = new EditContext(Ficha);
        //}

        //// Disparar carga asíncrona sin bloquear el render
        //_ = LoadDataAsync();

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

        // Cargar si es la primera vez o si el informante no se ha sincronizado correctamente
        if (!isInitialized || (Ficha.Id != 0 && Ficha.InformanteId > 0 && (informante == null || informante.Id != Ficha.InformanteId)))
        {
            await LoadDataAsync();
        }
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
            var tareaEncuestas = LoadEncuestasAutorizadasDisponiblesAsync();

            await Task.WhenAll(tareaDiccionarios, tareaPersonal, tareaDepartamentos, tareaEncuestas);

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

                if (Ficha.Informante != null)
                {
                    informante = Ficha.Informante;
                }
                else if (Ficha.InformanteId > 0)
                {
                    informante = await GetPersonaDetails(Ficha.InformanteId);
                }

                //if (Ficha.InformanteId > 0 && (informante == null || informante.Id != Ficha.InformanteId))
                //{
                //    informante = await GetPersonaDetails(Ficha.InformanteId);
                //}

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
                                // await LoadSectoresAsync(selectedMunicipio.Id);

                                // 6. Seleccionar el sector correcto
                                //if (Ficha.SectorId > 0)
                                //{
                                //    selectedSector = sectores?.FirstOrDefault(s => s.Id == Ficha.SectorId);
                                //}

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

    private async Task LoadEncuestasAutorizadasDisponiblesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<EncuestaAutorizada>>("/api/encuestasAutorizadas/disponibles");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        encuestasAutorizadasDisponibles = responseHttp.Response;
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

    //private async Task LoadSectoresAsync(int municipioId)
    //{
    //    var responseHttp = await Repository.GetAsync<List<Sector>>($"/api/sectores/combo/{municipioId}");
    //    if (responseHttp.Error)
    //    {
    //        var message = await responseHttp.GetErrorMessageAsync();
    //        Snackbar.Add(message!, Severity.Error);
    //        return;
    //    }
    //    sectores = responseHttp.Response;
    //}

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

    private void EncuestaAutorizadaChanged(EncuestaAutorizada encuestaAutorizada)
    {
        if (encuestaAutorizada == null)
            return;

        selectedEncuestaAutorizada = encuestaAutorizada;

        // Actualizar Departamento
        selectedDepartamento = encuestaAutorizada.Municipio?.Departamento;

        // Actualizar Municipio y su ID
        selectedMunicipio = encuestaAutorizada.Municipio;
        if (selectedMunicipio != null)
        {
            Ficha.MunicipioId = selectedMunicipio.Id;
        }

        // Actualizar Barrio/Comarca
        selectedBarrioComarca = encuestaAutorizada.BarrioComarca;
        if (selectedBarrioComarca != null)
        {
            Ficha.BarrioComarcaId = selectedBarrioComarca.Id;
        }

        // Actualizar Caserio
        selectedCaserio = encuestaAutorizada.Caserio;
        if (selectedCaserio != null)
        {
            Ficha.CaserioId = selectedCaserio.Id;
        }

        // Establecer el Tipo de Sector
        Ficha.TipoSector = encuestaAutorizada.TipoSector;

        // ASIGNAR EL CÓDIGO DE ENCUESTA A LA FICHA
        Ficha.CodEncuesta = encuestaAutorizada.CodEncuesta;

        // Determinar el Tipo de Encuesta según el código
        if (encuestaAutorizada.CodEncuesta.Length == 17)
        {
            Ficha.TipoEncuesta = TipoEncuesta.Unificada;
        }
        else
        {
            Ficha.TipoEncuesta = TipoEncuesta.Horizontal;
        }

        // Notificar al formulario que hubo cambios
        editContext?.NotifyFieldChanged(FieldIdentifier.Create(() => Ficha.MunicipioId));
        editContext?.NotifyFieldChanged(FieldIdentifier.Create(() => Ficha.CodEncuesta));

        StateHasChanged();
    }

    //private void EncuestaAutorizadaChanged(EncuestaAutorizada encuestaAutorizada)
    //{
    //    if (encuestaAutorizada == null)
    //        return;

    //    selectedEncuestaAutorizada = encuestaAutorizada;
    //    selectedDepartamento = encuestaAutorizada.Municipio!.Departamento;
    //    selectedMunicipio = encuestaAutorizada.Municipio;
    //    selectedBarrioComarca = encuestaAutorizada.BarrioComarca;
    //    selectedCaserio = encuestaAutorizada.Caserio;
    //    Ficha.TipoSector = encuestaAutorizada.TipoSector;

    //    if (encuestaAutorizada.CodEncuesta.Length == 17)
    //    {
    //        Ficha.TipoEncuesta = TipoEncuesta.Unificada;
    //    }
    //}

    private void EncuestadorChanged(PersonalEncuesta encuestador)
    {
        //if (encuestador == null) return;
        selectedEncuestador = encuestador;
        Ficha.EncuestadorId = encuestador?.Id ?? 0;
    }

    private void TecnicoCatastralChanged(PersonalEncuesta tecnicoCatastral)
    {
        //if (tecnicoCatastral == null) return;
        selectedTecnicoCatastral = tecnicoCatastral;
        Ficha.TecnicoCatastralId = tecnicoCatastral?.Id ?? 0;
        //GenerarCodigoEncuesta();
    }

    private void SupervisorChanged(PersonalEncuesta supervisor)
    {
        //if (supervisor == null) return;
        selectedSupervisor = supervisor;
        Ficha.CoordinadorId = supervisor?.Id ?? 0;
    }

    private void UnidadMedidaChanged(Diccionario? unidadMedida)
    {
        selectedUnidadMedida = unidadMedida;
        Ficha.UnidadMedidaId = unidadMedida?.Id;
    }

    private void EstadoChanged(Diccionario estado)
    {
        if (estado == null) return;
        selectedEstado = estado;
        Ficha.EstadoId = estado.Id;
    }

    private void OrigenTierraChanged(Diccionario? origenTierra)
    {
        selectedOrigenTierra = origenTierra;
        Ficha.OrigenTierraId = origenTierra?.Id;
    }

    //private void RelacionInformantePropietarioChanged(Diccionario? relacionInformantePropietario)
    //{
    //    selectedRelacionInformantePropietario = relacionInformantePropietario;
    //    Ficha.RelacionInformantePropietarioId = relacionInformantePropietario?.Id;
    //}

    private void ServidumbreAguaChanged(Diccionario? servidumbreAgua)
    {
        selectedServidumbreAgua = servidumbreAgua;
        Ficha.ServidumbreAguaId = servidumbreAgua?.Id;
    }

    private void ServidumbrePaseChanged(Diccionario? servidumbrePase)
    {
        selectedServidumbrePase = servidumbrePase;
        Ficha.ServidumbrePaseId = servidumbrePase?.Id;
    }

    private void ServidumbreOtraChanged(Diccionario? servidumbreOtra)
    {
        selectedServidumbreOtra = servidumbreOtra;
        Ficha.ServidumbreOtraId = servidumbreOtra?.Id;
    }

    private async Task DepartamentoChangedAsync(Departamento departamento)
    {
        if (departamento == null)
            return;

        selectedDepartamento = departamento;
        selectedMunicipio = null;
        //selectedSector = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        municipios = null;
        //sectores = null;
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
        //selectedSector = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        //sectores = null;
        barriosComarcas = null;
        caserios = null;

        // Limpiar los IDs
        //Ficha.SectorId = 0;
        Ficha.BarrioComarcaId = null;
        Ficha.CaserioId = null;

        //await LoadSectoresAsync(municipio.Id);
        await LoadBarriosComarcasAsync(municipio.Id);
        //GenerarCodigoEncuesta();

        editContext?.NotifyFieldChanged(FieldIdentifier.Create(() => Ficha.MunicipioId));

        StateHasChanged();
    }

    //private void SectorChanged(Sector sector)
    //{
    //    if (sector == null)
    //        return;

    //    selectedSector = sector;
    //    Ficha.SectorId = sector.Id;
    //    GenerarCodigoEncuesta();
    //}

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

    private void CaserioChanged(Caserio? caserio)
    {
        selectedCaserio = caserio;
        Ficha.CaserioId = caserio?.Id;
    }

    private void RelacionInformanteParcelaChanged(Diccionario? relacionInformanteParcela)
    {
        selectedRelacionInformanteParcela = relacionInformanteParcela;
        Ficha.RelacionInformanteParcelaId = relacionInformanteParcela?.Id;
    }

    private void RelacionInformantePropietarioChanged(Diccionario? relacionInformantePropietario)
    {
        selectedRelacionInformantePropietario = relacionInformantePropietario;
        Ficha.RelacionInformantePropietarioId = relacionInformantePropietario?.Id;

        // Solo auto-seleccionar si la ficha es nueva
        if (Ficha.Id == 0 && relacionInformantePropietario != null && !string.IsNullOrWhiteSpace(relacionInformantePropietario.Nombre))
        {
            var nombreLower = relacionInformantePropietario.Nombre.ToLower();
            if (nombreLower.Contains("mismo") || nombreLower.Equals("propietario"))
            {
                esInformantePropietario = true;
            }
        }
    }

    private async Task HandleValidSubmitInternal()
    {
        // Indicar si es una creación inicial antes de llamar a OnValidSubmit
        bool esNuevaFicha = Ficha.Id == 0;

        // 1. Ejecutar el guardado/actualización de la Ficha en la API
        await OnValidSubmit.InvokeAsync();

        // 2. Si era una ficha nueva, estaba marcada la casilla y se asignó un Id válido
        if (esNuevaFicha && esInformantePropietario && Ficha.Id > 0 && Ficha.InformanteId > 0)
        {
            await AgregarInformanteComoPropietarioAsync();

            // Resetear la variable local para que no vuelva a procesarse en futuros guardados
            esInformantePropietario = false;
        }
    }

    private async Task AgregarInformanteComoPropietarioAsync()
    {
        try
        {
            var nuevoPropietario = new Propietario
            {
                FichaId = Ficha.Id,
                PersonaId = Ficha.InformanteId,
                TipoDerecho = TipoDerecho.Propietario
            };

            var responseHttp = await Repository.PostAsync("/api/propietarios", nuevoPropietario);

            if (responseHttp.Error)
            {
                var error = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add($"Ficha guardada, pero no se pudo asociar como propietario: {error}", Severity.Warning);
            }
            else
            {
                Snackbar.Add("Informante agregado automáticamente como propietario.", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al registrar propietario: {ex.Message}", Severity.Error);
        }
    }

    private async Task<IEnumerable<EncuestaAutorizada>> SearchEncuestaAutorizada(string searchText, CancellationToken token)
    {
        await Task.Delay(5);

        if (encuestasAutorizadasDisponibles == null || !encuestasAutorizadasDisponibles.Any())
            return new List<EncuestaAutorizada>();

        if (string.IsNullOrWhiteSpace(searchText))
            return encuestasAutorizadasDisponibles.Take(10);

        return encuestasAutorizadasDisponibles
            .Where(e => e.CodEncuesta.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ||
                       (e.Municipio?.Nombre?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                       (e.BarrioComarca?.Nombre?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .Take(10)
            .ToList();
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

        if (departamentos == null || !departamentos.Any())
            return new List<Departamento>();

        if (string.IsNullOrWhiteSpace(searchText))
            return departamentos!;

        return departamentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Municipio>> SearchMunicipio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);

        // Verificar si municipios es null o está vacío
        if (municipios == null || !municipios.Any())
            return new List<Municipio>();

        if (string.IsNullOrWhiteSpace(searchText))
            return municipios!;

        return municipios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    //private async Task<IEnumerable<Sector>> SearchSector(string searchText, CancellationToken token)
    //{
    //    await Task.Delay(5);
    //    if (string.IsNullOrWhiteSpace(searchText))
    //        return sectores!;

    //    return sectores!
    //        .Where(c => c.NumeroSector.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
    //        .ToList();
    //}

    private async Task<IEnumerable<BarrioComarca>> SearchBarrioComarca(string searchText, CancellationToken token)
    {
        await Task.Delay(5);

        if (barriosComarcas == null || !barriosComarcas.Any())
            return new List<BarrioComarca>();

        if (string.IsNullOrWhiteSpace(searchText))
            return barriosComarcas!;

        return barriosComarcas!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Caserio>> SearchCaserio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);

        if (caserios == null || !caserios.Any())
            return new List<Caserio>();

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
            var informanteResult = await GetPersonaDetails(personaSeleccionada.Id);

            if (informanteResult != null)
            {
                informante = informanteResult;
                Ficha.InformanteId = informante.Id;

                // Limpiar o resetear las selecciones previas al cambiar de informante
                selectedRelacionInformanteParcela = null;
                selectedRelacionInformantePropietario = null;
                Ficha.RelacionInformanteParcelaId = null;
                Ficha.RelacionInformantePropietarioId = null;
                esInformantePropietario = false;

                Snackbar.Add("Datos del entrevistado cargados con éxito.", Severity.Success);
            }
            else
            {
                Snackbar.Add("No se pudieron cargar los datos del entrevistado.", Severity.Warning);
            }

            StateHasChanged();
        }
    }

    //private async Task ShowModalPersonaSearchAsync()
    //{
    //    var options = new DialogOptions
    //    {
    //        CloseOnEscapeKey = true,
    //        CloseButton = true,
    //        NoHeader = true,
    //        MaxWidth = MaxWidth.Large,
    //        FullWidth = true
    //    };

    //    var parameters = new DialogParameters<PersonaSearch>
    //    {
    //        { x => x.SoloNaturales, true }
    //    };

    //    var dialog = await DialogService.ShowAsync<PersonaSearch>("Buscar Persona", parameters, options);
    //    var result = await dialog.Result;

    //    if (!result.Canceled && result.Data is Persona personaSeleccionada)
    //    {
    //        var informanteResult = await GetPersonaDetails(personaSeleccionada.Id);

    //        if (informanteResult != null)
    //        {
    //            informante = informanteResult;
    //            Ficha.InformanteId = informante.Id;
    //            Snackbar.Add("Datos del entrevistado cargados con éxito.", Severity.Success);
    //        }
    //        else
    //        {
    //            Snackbar.Add("No se pudieron cargar los datos del entrevistado.", Severity.Warning);
    //        }

    //        StateHasChanged();
    //    }
    //}

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

    //private void GenerarCodigoEncuesta()
    //{
    //    if (selectedTecnicoCatastral!.Id != 0 &&
    //        selectedMunicipio!.Id != 0 &&
    //        selectedSector!.Id != 0 &&
    //        (!string.IsNullOrWhiteSpace(Ficha.Consecutivo) && Ficha.Consecutivo.Length == 4))
    //    {
    //        string inicial = selectedSector?.NumeroSector?.Substring(0, 1).ToUpper() ?? "";

    //        string sector = inicial switch
    //        {
    //            "R" => "RUR",
    //            "U" => "URB",
    //            _ => "000"
    //        };
    //        string codMuni = selectedMunicipio.CodMuni;
    //        string codTecnico = selectedTecnicoCatastral.Codigo;
    //        string consecutivo = Ficha.Consecutivo;

    //        Ficha.CodEncuesta = $"{sector}{codMuni}{codTecnico}{codTecnico}{consecutivo}";
    //    }
    //    else
    //    {
    //        Ficha.CodEncuesta = string.Empty;
    //    }
    //}
}