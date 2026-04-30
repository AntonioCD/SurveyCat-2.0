using Microsoft.AspNetCore.Identity;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<User> GetUserAsync(string identificacion);

    Task<IdentityResult> AddUserAsync(User user, string password);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);
}