using Microsoft.AspNetCore.Components;

namespace SurveyCat.Frontend.Components.Shared;

public partial class CollapsibleSection
{
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string Icon { get; set; } = "";
    [Parameter] public bool IsCollapsed { get; set; } = false;
    [Parameter] public EventCallback<bool> IsCollapsedChanged { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    private async Task ToggleSection()
    {
        IsCollapsed = !IsCollapsed;
        await IsCollapsedChanged.InvokeAsync(IsCollapsed);
    }
}