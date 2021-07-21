using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class ClassEditor : ComponentBase
    {
        private class Tab
        {
            public FakeTypes.Method.DevMethod? DevMethod { get; set; }
        }

        [EditorRequired]
        [Parameter]
        public FakeTypes.Class.DevClass DevClass { get; set; } = null!;

        private ICollection<Tab> OpenedTabs = new List<Tab>();

        private string? CurrentSelectedTab { get; set; }

        public void OpenMethod(FakeTypes.Method.DevMethod devMethod)
        {
            if (!OpenedTabs.Any(x => x.DevMethod == devMethod))
            {
                OpenedTabs.Add(new Tab()
                {
                    DevMethod = devMethod,
                });

                if (devMethod.GraphDefinition == null) // we need to initialize an empty graph definition
                {
                    var graph = new Graph.DevGraphDefinition(Program.Project, devMethod.DeclaringType.TypeNamespaceAndName + "." + devMethod.Name, devMethod.DeclaringType, Core.Graph.DevGraphDefinitionType.Method, devMethod.Name);
                    graph.InitializeEmptyForMethod(devMethod);
                    devMethod.GraphDefinition = graph;
                }
            }

            CurrentSelectedTab = "method:" + devMethod.Name;

            StateHasChanged();
        }

    }
}
