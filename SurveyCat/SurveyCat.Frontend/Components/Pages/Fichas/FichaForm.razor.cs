using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Fichas;

public partial class FichaForm
{
    private EditContext editContext = null!;
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

    private Diccionario? selectedUnidadMedida = new();
    private Diccionario? selectedEstado = new();
    private Diccionario? selectedOrigenTierra = new();
    private Diccionario? selectedRelacionInformanteParcela = new();
    private Diccionario? selectedRelacionInformantePropietario = new();
    private Diccionario? selectedServidumbreAgua = new();
    private Diccionario? selectedServidumbrePase = new();
    private Diccionario? selectedServidumbreOtra = new();
    private Departamento? selectedDepartamento = new();
    private Municipio? selectedMunicipio = new();
    private Sector? selectedSector = new();
    private BarrioComarca? selectedBarrioComarca = new();
    private Caserio? selectedCaserio = new();

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    [EditorRequired, Parameter] public Ficha Ficha { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override async Task OnInitializedAsync()
    {
        editContext = new(Ficha);

        // Cargamos lo básico que siempre debe estar (Catálogos y Deptos)
        await LoadDiccionariosAsync();
        await LoadDepartamentosAsync();

        if (Ficha.Id != 0)
        {
            // 1. Cargas en cascada condicionales
            if (Ficha.MunicipioId != 0)
            {
                await LoadMunicipiosAsync(Ficha.MunicipioId);
                selectedMunicipio = Ficha.Municipio;

                // Cargamos el depto desde el municipio si existe
                selectedDepartamento = Ficha.Municipio?.Departamento;
            }

            if (Ficha.SectorId != 0)
            {
                await LoadSectoresAsync(Ficha.SectorId);
                selectedSector = Ficha.Sector;
            }

            if (Ficha.BarrioComarcaId.HasValue)
            {
                await LoadBarriosComarcasAsync(Ficha.BarrioComarcaId.Value);
                selectedBarrioComarca = Ficha.BarrioComarca;
            }

            if (Ficha.CaserioId.HasValue)
            {
                await LoadCaseriosAsync(Ficha.CaserioId.Value);
                selectedCaserio = Ficha.Caserio;
            }

            // 2. Asignación segura de objetos de diccionario
            // Usamos el operador ?. para que si es null, la variable selected también sea null
            selectedEstado = Ficha.Estado;
            selectedOrigenTierra = Ficha.OrigenTierra;
            selectedRelacionInformanteParcela = Ficha.RelacionInformanteParcela;
            selectedRelacionInformantePropietario = Ficha.RelacionInformantePropietario;
            selectedServidumbreAgua = Ficha.ServidumbreAgua;
            selectedServidumbrePase = Ficha.ServidumbrePase;
            selectedServidumbreOtra = Ficha.ServidumbreOtra;
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

    private void UnidadMedidaChanged(Diccionario unidadMedida)
    {
        selectedUnidadMedida = unidadMedida;
        Ficha.UnidadMedidaId = unidadMedida.Id;
    }

    private void EstadoChanged(Diccionario estado)
    {
        selectedEstado = estado;
        Ficha.EstadoId = estado.Id;
    }

    private void OrigenTierraChanged(Diccionario origenTierra)
    {
        selectedOrigenTierra = origenTierra;
        Ficha.OrigenTierraId = origenTierra.Id;
    }

    private void RelacionInformanteParcelaChanged(Diccionario relacionInformanteParcela)
    {
        selectedRelacionInformanteParcela = relacionInformanteParcela;
        Ficha.RelacionInformanteParcelaId = relacionInformanteParcela.Id;
    }

    private void RelacionInformantePropietarioChanged(Diccionario relacionInformantePropietario)
    {
        selectedRelacionInformantePropietario = relacionInformantePropietario;
        Ficha.RelacionInformantePropietarioId = relacionInformantePropietario.Id;
    }

    private void ServidumbreAguaChanged(Diccionario servidumbreAgua)
    {
        selectedServidumbreAgua = servidumbreAgua;
        Ficha.ServidumbreAguaId = servidumbreAgua.Id;
    }

    private void ServidumbrePaseChanged(Diccionario servidumbrePase)
    {
        selectedServidumbrePase = servidumbrePase;
        Ficha.ServidumbrePaseId = servidumbrePase.Id;
    }

    private void ServidumbreOtraChanged(Diccionario servidumbreOtra)
    {
        selectedServidumbreOtra = servidumbreOtra;
        Ficha.ServidumbreOtraId = servidumbreOtra.Id;
    }

    private async Task DepartamentoChangedAsync(Departamento departamento)
    {
        selectedDepartamento = departamento;
        selectedMunicipio = new Municipio();
        selectedBarrioComarca = new BarrioComarca();
        selectedCaserio = new Caserio();
        municipios = null;
        barriosComarcas = null;
        caserios = null;
        await LoadMunicipiosAsync(departamento.Id);
    }

    private async Task MunicipioChangedAsync(Municipio municipio)
    {
        selectedMunicipio = municipio;
        Ficha.MunicipioId = municipio.Id;
        selectedSector = new Sector();
        selectedBarrioComarca = new BarrioComarca();
        selectedCaserio = new Caserio();
        barriosComarcas = null;
        caserios = null!;
        await LoadSectoresAsync(municipio.Id);
        await LoadBarriosComarcasAsync(municipio.Id);
    }

    private void SectorChanged(Sector sector)
    {
        selectedSector = sector;
        Ficha.SectorId = sector.Id;
    }

    private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    {
        selectedBarrioComarca = barrioComarca;
        Ficha.BarrioComarcaId = barrioComarca.Id;
        selectedCaserio = new Caserio();
        caserios = null!;
        await LoadCaseriosAsync(barrioComarca.Id);
    }

    private void CaserioChanged(Caserio caserio)
    {
        selectedCaserio = caserio;
        Ficha.CaserioId = caserio.Id;
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

    private async Task<IEnumerable<Diccionario>> SearchEstado(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaEstado!;
        }

        return listaEstado!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchOrigenTierra(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaOrigenTierra!;
        }

        return listaOrigenTierra!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchRelacionInformanteParcela(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaRelacionInformanteParcela!;
        }

        return listaRelacionInformanteParcela!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchRelacionInformantePropietario(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaRelacionInformantePropietario!;
        }

        return listaRelacionInformantePropietario!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbreAgua(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaServidumbre!;
        }

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbrePase(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaServidumbre!;
        }

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Diccionario>> SearchServidumbreOtra(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return listaServidumbre!;
        }

        return listaServidumbre!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Departamento>> SearchDepartamento(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return departamentos!;
        }

        return departamentos!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Municipio>> SearchMunicipio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return municipios!;
        }

        return municipios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Sector>> SearchSector(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return sectores!;
        }

        return sectores!
            .Where(c => c.NumeroSector.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<BarrioComarca>> SearchBarrioComarca(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return barriosComarcas!;
        }

        return barriosComarcas!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Caserio>> SearchCaserio(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return caserios!;
        }

        return caserios!
            .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
}