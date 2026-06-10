using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Frontend.Services;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using System.Diagnostics.Metrics;

namespace SurveyCat.Frontend.Components.Pages.Auth;

public partial class Register
{
    private UserDTO userDTO = new();
    private List<PersonalEncuesta>? personalEncuestas;
    private bool loading;
    private string? imageUrl;
    private string? titleLabel;

    private PersonalEncuesta selectedPersonalEncuesta = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public bool IsAdmin { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadPersonalEncuestasAsync();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        titleLabel = IsAdmin ? "Registro de Administrador" : "Registro de Usuario";
    }

    private async Task LoadPersonalEncuestasAsync()
    {
        var responseHttp = await Repository.GetAsync<List<PersonalEncuesta>>("/api/personalEncuestas/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        personalEncuestas = responseHttp.Response!
        .Where(p => string.IsNullOrWhiteSpace(p.UserId))
        .ToList();
    }

    private void PersonalEncuestaChanged(PersonalEncuesta personalEncuesta)
    {
        selectedPersonalEncuesta = personalEncuesta;
    }

    private async Task<IEnumerable<PersonalEncuesta>> SearchPersonalEncuestas(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return personalEncuestas!;
        }

        return personalEncuestas!
            .Where(c => c.Persona!.NombreCompleto.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/users");
    }

    private void InvalidForm()
    {
        Snackbar.Add("Por favor llena todos los campos del formulario.", Severity.Warning);
    }

    private async Task CreateUserAsync()
    {
        if (selectedPersonalEncuesta?.Persona is null)
        {
            InvalidForm();
            return;
        }

        userDTO.Activo = true;
        userDTO.UserName = selectedPersonalEncuesta!.Persona!.Identificacion;
        userDTO.PersonalEncuesta = selectedPersonalEncuesta;

        //if (IsAdmin)
        //{
        //    userDTO.UserType = UserType.Administrador;
        //}

        loading = true;
        var responseHttp = await Repository.PostAsync<UserDTO, TokenDTO>("/api/accounts/CreateUser", userDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        //await LoginService.LoginAsync(responseHttp.Response!.Token);
        NavigationManager.NavigateTo("/users");
    }
}