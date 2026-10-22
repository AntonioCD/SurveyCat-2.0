using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Jornadas;

public partial class JornadaForm
{
    private EditContext editContext = null!;

    [EditorRequired, Parameter] public Jornada Jornada { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        editContext = new(Jornada);
    }
}