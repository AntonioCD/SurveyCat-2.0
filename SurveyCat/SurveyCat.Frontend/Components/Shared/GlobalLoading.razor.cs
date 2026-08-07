using Microsoft.AspNetCore.Components;

namespace SurveyCat.Frontend.Components.Shared
{
    public partial class GlobalLoading
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string? Label { get; set; } = "Cargando...";
        [Parameter] public string? SubLabel { get; set; } = "Por favor espera";
    }
}