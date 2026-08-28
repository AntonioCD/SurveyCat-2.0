using ExcelDataReader;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SurveyCat.Frontend.Components.Pages.EncuestasAutorizadas;

public partial class EncuestasAutorizadasListUpload
{
    private bool loading = true;
    private bool procesandoArchivo = false;
    private double progress = 0;
    private List<Departamento>? departamentos;
    private List<Municipio>? municipios;
    private List<BarrioComarca>? barriosComarcas;
    private List<Caserio>? caserios;

    private TipoSector tipoSectorSeleccionado;
    private Departamento? selectedDepartamento;
    private Municipio? selectedMunicipio;
    private BarrioComarca? selectedBarrioComarca;
    private Caserio? selectedCaserio;
    private User? user;
    private string usuarioId = "SD";
    private IBrowserFile? selectedFile;

    private bool mostrarVistaPrevia = false;
    private List<VistaPreviaEncuesta> vistaPrevia = new();
    private List<string> codigosCargados = new();
    private string errorArchivo = string.Empty;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
        await LoadDepartamentosAsync();
        loading = false;
        StateHasChanged();
    }

    private async Task LoadUserAsync()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var authUser = authState.User;

            if (authUser.Identity is { IsAuthenticated: true })
            {
                // Obtener el username correctamente
                var userName = authUser.Identity?.Name
                               ?? authUser.FindFirst(ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(userName))
                {
                    // Buscar el usuario por nombre de usuario
                    var responseHttp = await Repository.GetAsync<User>($"/api/accounts");
                    if (responseHttp.Error)
                    {
                        if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            Snackbar.Add("Usuario no encontrado", Severity.Error);
                            return;
                        }
                        var messageError = await responseHttp.GetErrorMessageAsync();
                        Snackbar.Add(messageError!, Severity.Error);
                        return;
                    }

                    user = responseHttp.Response;
                    usuarioId = user!.Id; // Asignar el ID del usuario
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al cargar usuario: {ex.Message}", Severity.Error);
        }
    }

    private bool IsFormValid()
    {
        return (int)tipoSectorSeleccionado > 0 &&
               selectedDepartamento != null &&
               selectedMunicipio != null &&
               selectedBarrioComarca != null &&
               codigosCargados.Any();
    }

    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
        errorArchivo = string.Empty;
        codigosCargados.Clear();
        vistaPrevia.Clear();
        mostrarVistaPrevia = false;

        if (selectedFile != null)
        {
            await ProcesarArchivoAsync();
        }

        StateHasChanged();
    }

    private async Task ProcesarArchivoAsync()
    {
        if (selectedFile == null)
        {
            Snackbar.Add("No hay archivo seleccionado", Severity.Warning);
            return;
        }

        procesandoArchivo = true;
        progress = 0;
        StateHasChanged();

        try
        {
            errorArchivo = string.Empty;
            codigosCargados.Clear();
            vistaPrevia.Clear();
            mostrarVistaPrevia = false;

            // Configurar encoding para Excel
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream())
            {
                await selectedFile.OpenReadStream(maxAllowedSize: 10485760).CopyToAsync(stream);
                stream.Position = 0;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables[0];

                    if (table == null || table.Rows.Count == 0)
                    {
                        errorArchivo = "El archivo Excel está vacío";
                        return;
                    }

                    // Mostrar los nombres de columnas para depuración
                    Console.WriteLine("Columnas encontradas:");
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        Console.WriteLine($"Columna {col}: '{table.Columns[col].ColumnName}'");
                    }

                    // Buscar la columna que contenga "encuesta" (sin importar mayúsculas/minúsculas)
                    int columnaCodigo = -1;
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        var header = table.Columns[col].ColumnName?.Trim() ?? "";

                        // Buscar cualquier variación de "código encuesta"
                        if (!string.IsNullOrEmpty(header) &&
                            (header.Contains("encuesta", StringComparison.OrdinalIgnoreCase) ||
                             header.Contains("código", StringComparison.OrdinalIgnoreCase) ||
                             header.Contains("codigo", StringComparison.OrdinalIgnoreCase) ||
                             header.Contains("cód", StringComparison.OrdinalIgnoreCase) ||
                             header.Contains("cod", StringComparison.OrdinalIgnoreCase)))
                        {
                            columnaCodigo = col;
                            break;
                        }
                    }

                    // Si no encontró por nombre, usar la primera columna
                    if (columnaCodigo == -1 && table.Columns.Count > 0)
                    {
                        columnaCodigo = 0; // Usar la primera columna
                        Console.WriteLine("Usando la primera columna como fallback");
                    }

                    if (columnaCodigo == -1)
                    {
                        errorArchivo = "No se encontró una columna con códigos de encuesta en el archivo";
                        return;
                    }

                    // Leer los códigos
                    var codigos = new List<string>();
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        var codigo = table.Rows[row][columnaCodigo]?.ToString()?.Trim();

                        // Saltar encabezados vacíos o encabezados de columna
                        if (row == 0 && (string.IsNullOrEmpty(codigo) ||
                            codigo.Contains("encuesta", StringComparison.OrdinalIgnoreCase) ||
                            codigo.Contains("código", StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(codigo))
                        {
                            // Validar que el código tenga el formato correcto
                            // Ejemplo: URB01092943090951 (empieza con URB o RUR)
                            if (codigo.Length >= 10)
                            {
                                codigos.Add(codigo.ToUpper()); // Convertir a mayúsculas
                            }
                            else
                            {
                                // Si es muy corto, podría ser un código válido, lo agregamos igual
                                if (codigo.Length >= 5)
                                {
                                    codigos.Add(codigo.ToUpper());
                                }
                                else
                                {
                                    errorArchivo = $"El código '{codigo}' en la fila {row + 1} no parece ser un código válido";
                                    return;
                                }
                            }
                        }
                    }

                    if (!codigos.Any())
                    {
                        errorArchivo = "No se encontraron códigos válidos en el archivo";
                        return;
                    }

                    codigosCargados = codigos;
                    progress = 100;
                    Snackbar.Add($"Se cargaron {codigosCargados.Count} códigos correctamente", Severity.Success);
                }
            }
        }
        catch (Exception ex)
        {
            errorArchivo = $"Error al procesar el archivo: {ex.Message}";
            Snackbar.Add(errorArchivo, Severity.Error);
        }
        finally
        {
            procesandoArchivo = false;
            StateHasChanged();
        }
    }

    private void GenerarVistaPrevia()
    {
        if (!IsFormValid()) return;

        vistaPrevia.Clear();
        var index = 0;

        foreach (var codigo in codigosCargados)
        {
            index++;
            vistaPrevia.Add(new VistaPreviaEncuesta
            {
                Index = index,
                CodEncuesta = codigo,
                TipoSector = tipoSectorSeleccionado.ToString(),
                DepartamentoNombre = selectedDepartamento!.Nombre,
                MunicipioNombre = selectedMunicipio!.Nombre,
                BarrioComarcaNombre = selectedBarrioComarca!.Nombre,
                CaserioNombre = selectedCaserio?.Nombre ?? "SD"
            });
        }

        mostrarVistaPrevia = true;
        StateHasChanged();
    }

    private async Task GenerarMasivo()
    {
        if (!IsFormValid() || string.IsNullOrEmpty(usuarioId))
        {
            Snackbar.Add("Complete todos los campos requeridos", Severity.Warning);
            return;
        }

        try
        {
            var encuestas = new List<EncuestaAutorizada>();
            var tipoSector = tipoSectorSeleccionado;
            var fechaCarga = DateTime.Now;

            foreach (var codigo in codigosCargados)
            {
                var encuesta = new EncuestaAutorizada
                {
                    CodEncuesta = codigo,
                    TipoSector = tipoSector,
                    MunicipioId = selectedMunicipio!.Id,
                    BarrioComarcaId = selectedBarrioComarca!.Id,
                    CaserioId = selectedCaserio?.Id,
                    UsuarioCargaId = usuarioId,
                    Observacion = $"Carga masiva - {fechaCarga:dd/MM/yyyy HH:mm}"
                };
                encuestas.Add(encuesta);
            }

            // Enviar al backend
            var responseHttp = await Repository.PostAsync<object>("/api/encuestasAutorizadas/bulk", encuestas);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add($"Error: {message}", Severity.Error);
                return;
            }

            // Verificar si la respuesta tiene datos
            if (responseHttp.Response != null)
            {
                // Intentar deserializar la respuesta
                try
                {
                    var jsonResponse = responseHttp.Response.ToString();
                    // Si es un objeto anónimo, podemos acceder a sus propiedades
                    var success = jsonResponse!.Contains("success") || jsonResponse.Contains("Success");

                    MudDialog?.Close(DialogResult.Ok(true));
                    Snackbar.Add($"? Se cargaron {encuestas.Count} encuestas autorizadas exitosamente", Severity.Success);
                    NavigationManager.NavigateTo("/encuestasAutorizadas", forceLoad: true);
                }
                catch
                {
                    // Si no se puede deserializar, asumimos que fue exitoso
                    MudDialog?.Close(DialogResult.Ok(true));
                    Snackbar.Add($"? Se cargaron {encuestas.Count} encuestas autorizadas exitosamente", Severity.Success);
                    NavigationManager.NavigateTo("/encuestasAutorizadas", forceLoad: true);
                }
            }
            else
            {
                // Si la respuesta es null pero no hubo error, asumimos éxito
                MudDialog?.Close(DialogResult.Ok(true));
                Snackbar.Add($"? Se cargaron {encuestas.Count} encuestas autorizadas exitosamente", Severity.Success);
                NavigationManager.NavigateTo("/encuestasAutorizadas", forceLoad: true);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al cargar las encuestas: {ex.Message}", Severity.Error);
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
        if (departamento == null) return;

        selectedDepartamento = departamento;
        selectedMunicipio = null;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        municipios = null;
        barriosComarcas = null;
        caserios = null;
        LimpiarVistaPrevia();

        await LoadMunicipiosAsync(departamento.Id);
    }

    private async Task MunicipioChangedAsync(Municipio municipio)
    {
        if (municipio == null) return;

        selectedMunicipio = municipio;
        selectedBarrioComarca = null;
        selectedCaserio = null;
        barriosComarcas = null;
        caserios = null;
        LimpiarVistaPrevia();

        await LoadBarriosComarcasAsync(municipio.Id);
    }

    private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    {
        // 1. Limpiar siempre caserío y vista previa
        selectedCaserio = null;
        caserios = null;
        LimpiarVistaPrevia();

        // 2. Si se deseleccionó o limpió el campo
        if (barrioComarca == null)
        {
            selectedBarrioComarca = null;
            return;
        }

        selectedBarrioComarca = barrioComarca;

        // 3. Solo consultar caseríos si es Comarca (EsBarrio == false)
        if (!barrioComarca.EsBarrio)
        {
            await LoadCaseriosAsync(barrioComarca.Id);
        }
    }

    //private async Task BarrioComarcaChangedAsync(BarrioComarca barrioComarca)
    //{
    //    if (barrioComarca == null) return;

    //    selectedBarrioComarca = barrioComarca;
    //    selectedCaserio = null;
    //    caserios = null;
    //    LimpiarVistaPrevia();

    //    await LoadCaseriosAsync(barrioComarca.Id);
    //}

    private void CaserioChanged(Caserio caserio)
    {
        selectedCaserio = caserio;
        LimpiarVistaPrevia();
    }

    private void LimpiarVistaPrevia()
    {
        mostrarVistaPrevia = false;
        vistaPrevia.Clear();
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

    private void Cancelar()
    {
        MudDialog?.Cancel();
    }
}

public class VistaPreviaEncuesta
{
    public int Index { get; set; }
    public string CodEncuesta { get; set; } = string.Empty;
    public string TipoSector { get; set; } = string.Empty;
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string MunicipioNombre { get; set; } = string.Empty;
    public string BarrioComarcaNombre { get; set; } = string.Empty;
    public string? CaserioNombre { get; set; }
}