using Microsoft.AspNetCore.Identity;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.UnitsOfWork.Implementations;

public class UsersUnitOfWork : IUsersUnitOfWork
{
    private readonly IUsersRepository _usersRepository;

    public UsersUnitOfWork(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password) => await _usersRepository.AddUserAsync(user, password);

    public async Task AddUserToRoleAsync(User user, string roleName) => await _usersRepository.AddUserToRoleAsync(user, roleName);

    public async Task CheckRoleAsync(string roleName) => await _usersRepository.CheckRoleAsync(roleName);

    public async Task<User> GetUserAsync(string identificacion) => await _usersRepository.GetUserAsync(identificacion);

    public async Task<bool> IsUserInRoleAsync(User user, string roleName) => await _usersRepository.IsUserInRoleAsync(user, roleName);
}