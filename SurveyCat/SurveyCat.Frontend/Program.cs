using MudBlazor.Services;
using SurveyCat.Frontend.Components;
using SurveyCat.Frontend.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7258") });
builder.Services.AddScoped<IRepository, Repository>();

// Inicio: Para ver detalle de Errores
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
// Fin

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();