using DevEngine.Core.Graph;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.UI.Controls
{
    public partial class GraphArea : ComponentBase
    {

        [Parameter]
        public IDevGraphDefinition DevGraphDefinition { get; set; }

        public readonly IDictionary<IDevGraphNode, GraphNode> Nodes = new Dictionary<IDevGraphNode, GraphNode>();


        private System.Drawing.PointF? GetParameterAbsolutePosition(IDevGraphNodeParameter devGraphNodeParameter)
        {
            if (!Nodes.TryGetValue(devGraphNodeParameter.ParentNode, out var node))
                return null;

            return node.GetParameterAbsolutePosition(devGraphNodeParameter);
        }

        private string GetSvgPath(System.Drawing.PointF a, System.Drawing.PointF b)
        {
            var diff = new System.Drawing.PointF(b.X - a.X, b.Y - a.Y);

            var pathStr = "M" + a.X + "," + a.Y + " ";
            pathStr += "C";
            pathStr += a.X + diff.X / 3 * 2 + "," + a.Y + " ";
            pathStr += a.X + diff.X / 3 + "," + b.Y + " ";
            pathStr += b.X + "," + b.Y;

            return pathStr;
        }

        public void GraphNodeMoved()
        {
            StateHasChanged();
        }

    }
}