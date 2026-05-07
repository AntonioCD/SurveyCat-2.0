using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Repositories.Implementations;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public UsersRepository(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.PersonalEncuesta!)
            .ThenInclude(c => c.Persona)
            .FirstOrDefaultAsync(x => x.Id == userId.ToString());
        return user!;
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<SignInResult> LoginAsync(LoginDTO model)
    {
        return await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password, int personalEncuestaId)
    {
        user.PersonalEncuesta = null;

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            var perfil = await _context.PersonalEncuestas
                .Include(p => p.Persona)
                .FirstOrDefaultAsync(p => p.Id == personalEncuestaId);

            if (perfil != null)
            {
                perfil.UserId = user.Id;
                user.PersonalEncuesta = perfil;

                await _context.SaveChangesAsync();
            }
        }

        return result;
    }

    public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task CheckRoleAsync(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });
        }
    }

    public async Task<User> GetUserAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.PersonalEncuesta!)
            .ThenInclude(p => p.Persona)
            .FirstOrDefaultAsync(x => x.UserName == username);
        return user!;
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }
}