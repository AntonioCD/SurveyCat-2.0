using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.DTOs;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyCat.Backend.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IConfiguration _configuration;

    public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration)
    {
        _usersUnitOfWork = usersUnitOfWork;
        _configuration = configuration;
    }

    //[HttpPost("changePassword")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
    //    if (user == null)
    //    {
    //        return NotFound();
    //    }

    //    var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
    //    if (!result.Succeeded)
    //    {
    //        return BadRequest(result.Errors.FirstOrDefault()!.Description);
    //    }

    //    return NoContent();
    //}

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PutAsync(User user)
    {
        try
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.Activo = user.Activo;

            var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
            if (result.Succeeded)
            {
                return Ok(BuildToken(currentUser));
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
    {
        User user = model;

        var result = await _usersUnitOfWork.AddUserAsync(user, model.Password, model.PersonalEncuesta!.Id);
        if (result.Succeeded)
        {
            await _usersUnitOfWork.AddUserToRoleAsync(user, user.PersonalEncuesta!.TipoRol.ToString());
            return Ok(BuildToken(user));
        }

        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var result = await _usersUnitOfWork.LoginAsync(model);
        if (result.Succeeded)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Username);
            return Ok(BuildToken(user));
        }

        return BadRequest("Usuario o contraseña incorrectos.");
    }

    private TokenDTO BuildToken(User user)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Role, user.PersonalEncuesta!.TipoRol.ToString()),
                new("PrimerNombre", user.PersonalEncuesta.Persona!.PrimerNombre ?? user.PersonalEncuesta.Persona.NombreCompleto)
                //new("Identificacion", user.PersonalEncuesta.Persona!.Identificacion!),
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}