using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;
using SurveyCat.Backend.Repositories.Implementations;
using SurveyCat.Backend.Repositories.Interfaces;
using SurveyCat.Backend.UnitsOfWork.Implementations;
using SurveyCat.Backend.UnitsOfWork.Interfaces;
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
builder.Services.AddScoped<IDepartamentosRepository, DepartamentosRepository>();
builder.Services.AddScoped<IDiccionariosRepository, DiccionariosRepository>();
builder.Services.AddScoped<IMunicipiosRepository, MunicipiosRepository>();
builder.Services.AddScoped<IPersonasRepository, PersonasRepository>();

builder.Services.AddScoped<IBarriosComarcasUnitOfWork, BarriosComarcasUnitOfWork>();
builder.Services.AddScoped<ICaseriosUnitOfWork, CaseriosUnitOfWork>();
builder.Services.AddScoped<IDepartamentosUnitOfWork, DepartamentosUnitOfWork>();
builder.Services.AddScoped<IDiccionariosUnitOfWork, DiccionariosUnitOfWork>();
builder.Services.AddScoped<IMunicipiosUnitOfWork, MunicipiosUnitOfWork>();
builder.Services.AddScoped<IPersonasUnitOfWork, PersonasUnitOfWork>();

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