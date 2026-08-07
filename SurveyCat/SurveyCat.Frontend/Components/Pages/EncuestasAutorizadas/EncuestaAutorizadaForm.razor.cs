using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Threading.Tasks;

namespace SurveyCat.Frontend.Components.Pages.EncuestasAutorizadas;

public partial class EncuestaAutorizadaForm
{
    private EditContext editContext = null!;
    private bool loading = true;
    private bool isInitialized = false;
    private List<Departamento>? departamentos;
    private List<Municipio>? municipios;
    private List<BarrioComarca>? barriosComarcas;
    private List<Caserio>? caserios;

    private Departamento? selectedDepartamento;
    private Municipio? selectedMunicipio;
    private BarrioComarca? selectedBarrioComarca;
    private Caserio? selectedCaserio;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [EditorRequired, Parameter] public EncuestaAutorizada EncuestaAutorizada { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnParametersSet()
    {
        if (isInitialized)
            return;

        if (EncuestaAutorizada == null)
        {
            EncuestaAutorizada = new EncuestaAutorizada();
        }

        if (editContext == null || editContext.Model != EncuestaAutorizada)
        {
            editContext = new EditContext(EncuestaAutorizada);
        }

        // Disparar carga asíncrona sin bloquear el render
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        loading = true;

        try
        {
            // Cargar departamentos primero
            await LoadDepartamentosAsync();

            if (EncuestaAutorizada.Id != 0)
            {
                // Si es edición, cargar los datos relacionados
                await LoadRelatedDataForEditAsync();
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
            // 1. Obtener el Municipio completo desde el API
            if (EncuestaAutorizada.MunicipioId > 0)
            {
                var municipioResponse = await Repository.GetAsync<Municipio>($"/api/municipios/{EncuestaAutorizada.MunicipioId}");
                if (!municipioResponse.Error && municipioResponse.Response != null)
                {
                    var municipio = municipioResponse.Response;

                    // 2. Obtener el Departamento del Municipio
                    if (municipio.DepartamentoId > 0)
                    {
                        var deptoResponse = await Repository.GetAsync<Departamento>($"/api/departamentos/{municipio.DepartamentoId}");
                        if (!deptoResponse.Error && deptoResponse.Response != null)
                        {
                            selectedDepartamento = deptoResponse.Response;

                            // 3. Cargar los municipios del departamento seleccionado
                            await LoadMunicipiosAsync(selectedDepartamento.Id);

                            // 4. Seleccionar el municipio correcto de la lista
                            selectedMunicipio = municipios?.FirstOrDefault(m => m.Id == EncuestaAutorizada.MunicipioId);

                            if (selectedMunicipio != null)
                            {
                                // 5. Si tiene Barrio/Comarca, cargarlo
                                if (EncuestaAutorizada.BarrioComarcaId.HasValue && EncuestaAutorizada.BarrioComarcaId.Value > 0)
                                {
                                    await LoadBarriosComarcasAsync(selectedMunicipio.Id);
                                    selectedBarrioComarca = barriosComarcas?.FirstOrDefault(b => b.Id == EncuestaAutorizada.BarrioComarcaId.Value);

                                    if (selectedBarrioComarca != null)
                                    {
                                        // 6. Si tiene Caserio, cargarlo
                                        if (EncuestaAutorizada.CaserioId.HasValue && EncuestaAutorizada.CaserioId.Value > 0)
                                        {
                                            await LoadCaseriosAsync(selectedBarrioComarca.Id);
                                            selectedCaserio = caserios?.FirstOrDefault(c => c.Id == EncuestaAutorizada.CaserioId.Value);
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

    private async Task DepartamentoChangedAsync(Departamento departamento)
    {
        if (departamento == null)
            return;

        selectedDepartamento = departamento;
        selectedMunicipio = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        municipios = null;
        barriosComarcas = null;
        caserios = null;

        // Limpiar los IDs
        EncuestaAutorizada.MunicipioId = 0;
        EncuestaAutorizada.BarrioComarcaId = null;
        EncuestaAutorizada.CaserioId = null;

        await LoadMunicipiosAsync(departamento.Id);
    }

    private async Task MunicipioChangedAsync(Municipio municipio)
    {
        if (municipio == null)
            return;

        selectedMunicipio = municipio;
        EncuestaAutorizada.MunicipioId = municipio.Id;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        barriosComarcas = null;
        caserios = null;

        // Limpiar los IDs
        EncuestaAutorizada.BarrioComarcaId = null;
        EncuestaAutorizada.CaserioId = null;

        await LoadBarriosComarcasAsync(municipio.Id);
    }

    private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    {
        if (barrioComarca == null)
            return;

        selectedBarrioComarca = barrioComarca;
        EncuestaAutorizada.BarrioComarcaId = barrioComarca.Id;
        selectedCaserio = null;
        caserios = null;

        // Limpiar el ID
        EncuestaAutorizada.CaserioId = null;

        await LoadCaseriosAsync(barrioComarca.Id);
    }

    private void CaserioChanged(Caserio caserio)
    {
        if (caserio == null)
            return;

        selectedCaserio = caserio;
        EncuestaAutorizada.CaserioId = caserio.Id;
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
}