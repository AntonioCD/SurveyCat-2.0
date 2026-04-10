using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SurveyCat.Shared.Entities;

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