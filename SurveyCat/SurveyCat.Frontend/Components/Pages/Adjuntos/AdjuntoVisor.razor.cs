using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace SurveyCat.Frontend.Components.Pages.Adjuntos;

public partial class AdjuntoVisor
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string DocumentoUrl { get; set; } = string.Empty;
    [Parameter] public string NombreArchivo { get; set; } = string.Empty;

    private string containerId = $"viewer-{Guid.NewGuid().ToString("N")}";
    private double zoomScale = 1.0;

    private void ZoomIn() { if (zoomScale < 3.5) zoomScale += 0.25; }
    private void ZoomOut() { if (zoomScale > 0.5) zoomScale -= 0.25; }
    private void ResetZoom() => zoomScale = 1.0;

    // Inicializamos el script de arrastre una vez que el contenedor ya existe en el HTML
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !DocumentoUrl.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            await JSRuntime.InvokeVoidAsync("inicializarArrastre", containerId);
        }
    }

    private async Task ToggleFullscreen()
    {
        await JSRuntime.InvokeVoidAsync("eval", $"let el = document.getElementById('{containerId}'); if(el) {{ if(!document.fullscreenElement) el.requestFullscreen(); else document.exitFullscreen(); }}");
    }

    private void Cerrar() => MudDialog.Close();
}