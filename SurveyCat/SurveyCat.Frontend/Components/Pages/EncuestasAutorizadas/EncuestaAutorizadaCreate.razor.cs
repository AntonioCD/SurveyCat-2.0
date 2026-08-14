using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;
using System.Net;
using System.Security.Claims;

namespace SurveyCat.Frontend.Components.Pages.EncuestasAutorizadas;

public partial class EncuestaAutorizadaCreate
{
    private EncuestaAutorizada encuestaAutorizada = new();
    private User? user;
    private string usuarioId = "SD";

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
        encuestaAutorizada.UsuarioCargaId = usuarioId;
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

    private async Task CreateAsync()
    {
        // Ya no necesitas asignar el UsuarioCargaId aquí porque ya se asignó en el formulario
        var responseHttp = await Repository.PostAsync("/api/encuestasAutorizadas", encuestaAutorizada);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add("Registro creado", Severity.Success);
    }

    private void Return()
    {
        NavigationManager.NavigateTo("/encuestasAutorizadas");
    }
}