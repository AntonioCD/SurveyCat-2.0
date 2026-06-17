using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SurveyCat.Frontend.Components.Pages.Auth;

namespace SurveyCat.Frontend.Components.Shared;

public partial class AuthLinks
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var authenticationState = await AuthenticationStateTask;
        var claims = authenticationState.User.Claims.ToList();
        var nameClaim = claims.FirstOrDefault(x => x.Type == "PrimerNombre");
    }

    private async Task ChangePassword()
    {
        var options = new DialogOptions()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            BackdropClick = false // Evita que se cierre por error si hacen clic afuera
        };

        var dialog = await DialogService.ShowAsync<ChangePassword>("Cambiar Contraseña", options);
        // Opcional: puedes esperar el resultado si lo necesitas
        // var result = await dialog.Result;
    }

    private void ShowModalLogIn()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.ShowAsync<Login>("Inicio de Sesión", closeOnEscapeKey);
    }

    private void ShowModalLogOut()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.ShowAsync<Logout>("Cerrar Sesión", closeOnEscapeKey);
    }
}