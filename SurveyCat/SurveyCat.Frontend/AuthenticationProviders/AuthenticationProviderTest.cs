using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SurveyCat.Frontend.AuthenticationProviders;

public class AuthenticationProviderTest : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(1000);

        var anonimous = new ClaimsIdentity();
        var admin = new ClaimsIdentity(
        [
            new("FirstName", "Oscar"),
            new("LastName", "Castellon"),
            new(ClaimTypes.Name, "0011404850007U"),
            new(ClaimTypes.Role, "Administrador")
        ],
        authenticationType: "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(admin)));
    }
}