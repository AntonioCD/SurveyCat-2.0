using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
using SurveyCat.Shared.Entities;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddTransient<SeedDb>();

builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IBarriosComarcasRepository, BarriosComarcasRepository>();
builder.Services.AddScoped<ICaseriosRepository, CaseriosRepository>();
builder.Services.AddScoped<IColindantesRepository, ColindantesRepository>();
builder.Services.AddScoped<IConflictosRepository, ConflictosRepository>();
builder.Services.AddScoped<IDepartamentosRepository, DepartamentosRepository>();
builder.Services.AddScoped<IDiccionariosRepository, DiccionariosRepository>();
builder.Services.AddScoped<IDocumentosAnexosRepository, DocumentosAnexosRepository>();
builder.Services.AddScoped<IFamiliasRepository, FamiliasRepository>();
builder.Services.AddScoped<IFichasRepository, FichasRepository>();
builder.Services.AddScoped<IMunicipiosRepository, MunicipiosRepository>();
builder.Services.AddScoped<IPersonalEncuestasRepository, PersonalEncuestasRepository>();
builder.Services.AddScoped<IPersonasRepository, PersonasRepository>();
builder.Services.AddScoped<IPropietariosRepository, PropietariosRepository>();
builder.Services.AddScoped<ISectoresRepository, SectoresRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IBarriosComarcasUnitOfWork, BarriosComarcasUnitOfWork>();
builder.Services.AddScoped<ICaseriosUnitOfWork, CaseriosUnitOfWork>();
builder.Services.AddScoped<IColindantesUnitOfWork, ColindantesUnitOfWork>();
builder.Services.AddScoped<IConflictosUnitOfWork, ConflictosUnitOfWork>();
builder.Services.AddScoped<IDepartamentosUnitOfWork, DepartamentosUnitOfWork>();
builder.Services.AddScoped<IDiccionariosUnitOfWork, DiccionariosUnitOfWork>();
builder.Services.AddScoped<IDocumentosAnexosUnitOfWork, DocumentosAnexosUnitOfWork>();
builder.Services.AddScoped<IFamiliasUnitOfWork, FamiliasUnitOfWork>();
builder.Services.AddScoped<IFichasUnitOfWork, FichasUnitOfWork>();
builder.Services.AddScoped<IMunicipiosUnitOfWork, MunicipiosUnitOfWork>();
builder.Services.AddScoped<IPersonalEncuestasUnitOfWork, PersonalEncuestasUnitOfWork>();
builder.Services.AddScoped<IPropietariosUnitOfWork, PropietariosUnitOfWork>();
builder.Services.AddScoped<IPersonasUnitOfWork, PersonasUnitOfWork>();
builder.Services.AddScoped<ISectoresUnitOfWork, SectoresUnitOfWork>();
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = false;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
        ClockSkew = TimeSpan.Zero
    });

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddConsole();

var app = builder.Build();
SeedData(app);

void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

if (app.Environment.IsDevelopment())
{
    // Esto expone el JSON en /openapi/v1.json
    app.MapOpenApi();

    // Esto agrega la interfaz gráfica de Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SurveyCat API v1");
        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();