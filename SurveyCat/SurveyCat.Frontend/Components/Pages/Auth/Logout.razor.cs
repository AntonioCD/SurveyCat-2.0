using Microsoft.AspNetCore.Components;
using MudBlazor;
using SurveyCat.Frontend.Services;

namespace SurveyCat.Frontend.Components.Pages.Auth;

public partial class Logout
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private async Task LogoutActionAsync()
    {
        await LoginService.LogoutAsync();
        NavigationManager.NavigateTo("/", forceLoad: false);
        CancelAction();
    }

    private void CancelAction()
    {
        MudDialog.Cancel();
    }
}