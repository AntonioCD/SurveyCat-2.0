using Microsoft.AspNetCore.Components;

namespace SurveyCat.Frontend.Components.Shared;

public partial class FilterComponent
{

    [Parameter] public string FilterValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> FilterValueChanged { get; set; }
    [Parameter] public EventCallback<string> ApplyFilter { get; set; }

    private async Task CleanFilter()
    {
        FilterValue = string.Empty;
        await FilterValueChanged.InvokeAsync(FilterValue);
        await ApplyFilter.InvokeAsync(FilterValue);
    }

    private async Task OnFilterApply()
    {
        await FilterValueChanged.InvokeAsync(FilterValue);
        await ApplyFilter.InvokeAsync(FilterValue);
    }
}