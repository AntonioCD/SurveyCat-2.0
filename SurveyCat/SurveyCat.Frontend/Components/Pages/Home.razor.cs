using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.DTOs;

namespace SurveyCat.Frontend.Components.Pages;

public partial class Home
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IRepository Repository { get; set; } = default!;

    // Variable para almacenar la respuesta de la API
    private DashboardResponseDTO? dashboardData;

    private bool cargando = true;

    // Variables para el gráfico de MudBlazor
    public List<ChartSeries> Series = new List<ChartSeries>();

    public string[] XAxisLabels = Array.Empty<string>();

    protected override async Task OnInitializedAsync()
    {
        // 1. Llamar al backend
        var responseHttp = await Repository.GetAsync<DashboardResponseDTO>("api/dashboard");

        if (!responseHttp.Error)
        {
            // 2. Guardar los datos reales
            dashboardData = responseHttp.Response;

            // 3. Configurar el gráfico con los datos de la API
            if (dashboardData != null)
            {
                Series = new List<ChartSeries>
            {
                new ChartSeries()
                {
                    Name = "Fichas Levantadas",
                    Data = dashboardData.ValoresGrafico.ToArray()
                }
            };
                XAxisLabels = dashboardData.MesesGrafico.ToArray();
            }
        }

        cargando = false;
    }
}