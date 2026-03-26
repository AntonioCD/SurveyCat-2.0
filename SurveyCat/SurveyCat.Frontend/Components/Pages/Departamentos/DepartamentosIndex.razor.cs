using Microsoft.AspNetCore.Components;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Departamentos;

public partial class DepartamentosIndex
{
    [Inject] private IRepository Repository { get; set; } = null!;

    private List<Departamento>? departamentos;

    protected override async Task OnInitializedAsync()
    {
        var httpResult = await Repository.GetAsync<List<Departamento>>("/api/departamentos");
        departamentos = httpResult.Response;
    }
}