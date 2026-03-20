using Microsoft.EntityFrameworkCore;
using SurveyCat.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));

var app = builder.Build();

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