using Microsoft.AspNetCore.Identity;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Responses;

namespace SurveyCat.Backend.UnitsOfWork.Interfaces;

public interface IUsersUnitOfWork
{
    Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<User> GetUserAsync(Guid userId);

    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    Task<IdentityResult> UpdateUserAsync(User user);

    Task<SignInResult> LoginAsync(LoginDTO model);

    Task LogoutAsync();

    Task<User> GetUserAsync(string username);

    Task<IdentityResult> AddUserAsync(User user, string password, int personalEncuestaId);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);
}