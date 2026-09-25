using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.DTOs;

namespace SurveyCat.Frontend.Components.Pages;

public partial class Home : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IRepository Repository { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private DashboardResponseDTO? dashboardData;
    private bool cargando = true;
    private bool cargandoAuth = true;

    public List<ChartSeries> Series = new List<ChartSeries>();
    public string[] XAxisLabels = Array.Empty<string>();

    protected override async Task OnInitializedAsync()
    {
        // Suscribirse a los cambios de estado de autenticación (Login/Logout)
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

        await CargarDatosSegunAuth();
    }

    private async Task CargarDatosSegunAuth()
    {
        cargandoAuth = true;
        StateHasChanged();

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true && user.IsInRole("Administrador"))
        {
            var responseHttp = await Repository.GetAsync<DashboardResponseDTO>("api/dashboard");

            if (!responseHttp.Error && responseHttp.Response != null)
            {
                dashboardData = responseHttp.Response;

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

        cargandoAuth = false;
        cargando = false;
        StateHasChanged();
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        await InvokeAsync(async () =>
        {
            await CargarDatosSegunAuth();
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        // Desuscribir el evento para evitar fugas de memoria
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}