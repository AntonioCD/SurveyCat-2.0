using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Frontend.Components.Pages.Municipios
{
    public partial class MunicipioForm
    {
        private EditContext editContext = null!;

        [EditorRequired, Parameter] public Municipio Municipio { get; set; } = null!;
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

        protected override void OnInitialized()
        {
            editContext = new(Municipio);
        }
    }
}