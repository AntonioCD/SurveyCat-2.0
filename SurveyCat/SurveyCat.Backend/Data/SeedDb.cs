using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.Entities;
using SurveyCat.Shared.Enums;
using System.Collections.Generic;

namespace SurveyCat.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        // Asegura que la base de datos y tablas existan
        await _context.Database.EnsureCreatedAsync();

        // 1. Cargar catálogos geográficos
        await CheckDepartamentosFullAsync();

        // 2. Cargar Roles
        await CheckRolesAsync();

        // 3. Crear Usuario, Perfil (PersonalEncuesta) y Persona
        await CheckUserAsync("0011404850007U", "OSCAR", "ANTONIO", "CASTELLÓN", "DELGADO", TipoRol.Administrador);
    }

    private async Task CheckDepartamentosFullAsync()
    {
        if (!_context.Departamentos.Any())
        {
            var scriptPath = Path.Combine("Data", "Dpt-Mun-Bar-Com-Cas-Sec.sql");
            if (File.Exists(scriptPath))
            {
                var sqlScript = await File.ReadAllTextAsync(scriptPath);
                await _context.Database.ExecuteSqlRawAsync(sqlScript);
            }
        }
    }

    private async Task CheckRolesAsync()
    {
        // Crea todos los roles definidos en el Enum
        foreach (TipoRol rol in Enum.GetValues(typeof(TipoRol)))
        {
            await _usersUnitOfWork.CheckRoleAsync(rol.ToString());
        }
    }

    private async Task<User> CheckUserAsync(
        string identificacion,
        string pNombre,
        string sNombre,
        string pApellido,
        string sApellido,
        TipoRol tipoRol)
    {
        // 1. Verificar si el Usuario existe en Identity
        var user = await _usersUnitOfWork.GetUserAsync(identificacion);

        // 2. Buscar o crear la Persona (La entidad legal)
        var persona = await _context.Personas
            .FirstOrDefaultAsync(p => p.Identificacion == identificacion);

        if (persona == null)
        {
            persona = new Persona
            {
                TipoPersona = TipoPersona.Natural,
                TipoIdentificacionId = 821,
                Identificacion = identificacion,
                PrimerNombre = pNombre,
                SegundoNombre = sNombre,
                PrimerApellido = pApellido,
                SegundoApellido = sApellido,
                // El setter de tu entidad probablemente ya arma el NombreCompleto
                NombreCompleto = $"{pNombre} {sNombre} {pApellido} {sApellido}".Replace("  ", " ").Trim()
            };
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
        }

        // 3. Buscar o crear el Perfil (PersonalEncuesta)
        var personalEncuesta = await _context.PersonalEncuestas
            .FirstOrDefaultAsync(p => p.PersonaId == persona.Id);

        if (personalEncuesta == null)
        {
            personalEncuesta = new PersonalEncuesta
            {
                PersonaId = persona.Id,
                Codigo = "000",
                Brigada = "000",
                TipoRol = tipoRol,
                UserId = null // Se mantiene null inicialmente
            };
            _context.PersonalEncuestas.Add(personalEncuesta);
            await _context.SaveChangesAsync();
        }

        // 4. Si el usuario no existe en Identity, lo creamos
        if (user == null)
        {
            user = new User
            {
                UserName = identificacion,
                Activo = true
            };

            // Se usa el UnitOfWork para manejar la creación y el password hash
            await _usersUnitOfWork.AddUserAsync(user, "123456", personalEncuesta.Id);
            await _usersUnitOfWork.AddUserToRoleAsync(user, user.PersonalEncuesta!.TipoRol.ToString());
        }

        // 5. Vincular PersonalEncuesta con el User recién creado (o existente)
        // Solo actualizamos si el UserId es diferente para evitar updates innecesarios
        if (personalEncuesta.UserId != user.Id)
        {
            personalEncuesta.UserId = user.Id;
            _context.PersonalEncuestas.Update(personalEncuesta);
            await _context.SaveChangesAsync();
        }

        return user;
    }
}