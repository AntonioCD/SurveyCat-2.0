using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Personas
{
    public partial class PersonaEdit
    {
        private Persona? persona;
        private List<Diccionario>? diccionarios;
        private List<Diccionario> listaEstadoCivil = new();
        private List<Diccionario> listaProfesion = new();
        private List<Diccionario> listaTipoIdentificacion = new();
        private List<Diccionario> listaTipoPersonaJuridica = new();
        private List<Departamento>? departamentos;
        private List<Municipio>? municipios;
        private List<BarrioComarca>? barriosComarcas;
        private List<Caserio>? caserios;
        private bool loading = true;

        private Diccionario selectedTipoIdentificacion = new();
        private Diccionario selectedEstadoCivil = new();
        private Diccionario selectedProfesion = new();
        private Diccionario selectedTipoPersonaJuridica = new();
        private Departamento selectedDepartamento = new();
        private Municipio selectedMunicipio = new();
        private BarrioComarca selectedBarrioComarca = new();
        private Caserio selectedCaserio = new();

        public IMask maskIdentificacion = new RegexMask(@"^[a-zA-Z0-9]*$");

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadPersonaAsync();
            await LoadDiccionariosAsync();
            await LoadDepartamentosAsync();

            if (persona!.Municipio != null)
            {
                await LoadMunicipiosAsync(persona.Municipio!.DepartamentoId);
                await LoadBarriosComarcasAsync(persona.MunicipioId);
            }
            if (persona.BarrioComarcaId != null)
            {
                await LoadCaseriosAsync(persona.BarrioComarcaId);
            }

            selectedTipoIdentificacion = persona.TipoIdentificacion!;
            selectedEstadoCivil = persona.EstadoCivil!;
            selectedProfesion = persona.Profesion!;
            selectedTipoPersonaJuridica = persona.TipoPersonaJuridica!;
            selectedDepartamento = persona.Municipio!.Departamento!;
            selectedMunicipio = persona.Municipio!;
            selectedBarrioComarca = persona.BarrioComarca!;
            selectedCaserio = persona.Caserio!;
        }

        private async Task LoadPersonaAsync()
        {
            var responseHttp = await Repository.GetAsync<Persona>($"api/personas/{Id}");

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("personas");
                }
                else
                {
                    var messageError = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messageError!, Severity.Error);
                }
            }
            else
            {
                persona = responseHttp.Response;
            }

            loading = false;
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
                listaEstadoCivil = diccionarios.Where(x => x.Catalogo == Catalogos.EstadoCivil).ToList();
                listaProfesion = diccionarios.Where(x => x.Catalogo == Catalogos.Profesion).ToList();
                listaTipoIdentificacion = diccionarios.Where(x => x.Catalogo == Catalogos.TipoIdentificacion).ToList();
                listaTipoPersonaJuridica = diccionarios.Where(x => x.Catalogo == Catalogos.TipoPersonaJuridica).ToList();
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

        private async Task LoadBarriosComarcasAsync(int? municipioId)
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

        private async Task LoadCaseriosAsync(int? comarcaId)
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

        private void TipoIdentificacionChanged(Diccionario tipoIdentificacion)
        {
            selectedTipoIdentificacion = tipoIdentificacion;
            persona!.TipoIdentificacionId = tipoIdentificacion.Id;
        }

        private void EstadoCivilChanged(Diccionario estadoCivil)
        {
            selectedEstadoCivil = estadoCivil;
            persona!.EstadoCivilId = estadoCivil.Id;
        }

        private void ProfesionChanged(Diccionario profesion)
        {
            selectedProfesion = profesion;
            persona!.ProfesionId = profesion.Id;
        }

        private void TipoPersonaJuridicaChanged(Diccionario tipoPersonaJuridica)
        {
            selectedTipoPersonaJuridica = tipoPersonaJuridica;
            persona!.TipoPersonaJuridicaId = tipoPersonaJuridica.Id;
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
            selectedBarrioComarca = new BarrioComarca();
            selectedCaserio = new Caserio();
            barriosComarcas = null;
            caserios = null!;
            await LoadBarriosComarcasAsync(municipio.Id);
        }

        private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
        {
            selectedBarrioComarca = barrioComarca;
            selectedCaserio = new Caserio();
            caserios = null!;
            await LoadCaseriosAsync(barrioComarca.Id);
        }

        private void CaserioChanged(Caserio caserio)
        {
            selectedCaserio = caserio;
            persona!.CaserioId = caserio.Id;
        }

        private async Task<IEnumerable<Diccionario>> SearchTipoIdentificacion(string searchText, CancellationToken token)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return listaTipoIdentificacion!;
            }

            return listaTipoIdentificacion!
                .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Diccionario>> SearchEstadoCivil(string searchText, CancellationToken token)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return listaEstadoCivil!;
            }

            return listaEstadoCivil!
                .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Diccionario>> SearchProfesion(string searchText, CancellationToken token)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return listaProfesion!;
            }

            return listaProfesion!
                .Where(c => c.Nombre.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Diccionario>> SearchTipoPersonaJuridica(string searchText, CancellationToken token)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return listaTipoPersonaJuridica!;
            }

            return listaTipoPersonaJuridica!
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

        private async Task EditAsync()
        {
            var responseHttp = await Repository.PutAsync("api/personas", persona);

            if (responseHttp.Error)
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
                return;
            }

            Return();
            Snackbar.Add("Registro guardado.", Severity.Success);
        }

        private void Return()
        {
            NavigationManager.NavigateTo($"/personas");
        }
    }
}