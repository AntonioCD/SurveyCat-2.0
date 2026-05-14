using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Frontend.Services;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;
using System.Net;

namespace SurveyCat.Frontend.Components.Pages.Auth;

[Authorize]
public partial class EditUser
{
    private User? user;
    private bool loading = true;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;

    [Parameter] public Guid UserId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
    }

    [Obsolete]
    private void ShowModal()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<ChangePassword>("Cambiar Contraseña", closeOnEscapeKey);
    }

    private async Task LoadUserAsync()
    {
        var responseHttp = await Repository.GetAsync<User>($"/api/accounts/{UserId}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        user = responseHttp.Response;
        loading = false;
    }

    private async Task SaveUserAsync()
    {
        var responseHttp = await Repository.PutAsync<User, TokenDTO>("/api/accounts", user!);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);
        Snackbar.Add("Usuario modificado con éxito.", Severity.Success);
        NavigationManager.NavigateTo("/");
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/users");
    }

    private void InvalidForm()
    {
        Snackbar.Add("Por favor llena todos los campos del formulario.", Severity.Warning);
    }
}