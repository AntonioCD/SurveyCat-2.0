using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Caserios;

public partial class CaserioForm
{
    private EditContext editContext = null!;

    [EditorRequired, Parameter] public Caserio Caserio { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        editContext = new(Caserio);
    }
}