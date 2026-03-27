using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SurveyCat.Shared.Entities;
using System.Diagnostics.Metrics;

namespace SurveyCat.Frontend.Components.Pages.Departamentos
{
    public partial class DepartamentoForm
    {
        private EditContext editContext = null!;

        [EditorRequired, Parameter] public Departamento Departamento { get; set; } = null!;
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

        protected override void OnInitialized()
        {
            editContext = new(Departamento);
        }
    }
}