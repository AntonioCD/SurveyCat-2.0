using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SurveyCat.Frontend.Repositories;
using SurveyCat.Shared.Constants;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;

namespace SurveyCat.Frontend.Components.Pages.Diccionarios;

public partial class DiccionarioForm
{
    private EditContext editContext = null!;

    [EditorRequired, Parameter] public Diccionario Diccionario { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        editContext = new(Diccionario);
    }
}