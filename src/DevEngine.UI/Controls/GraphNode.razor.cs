using DevEngine.Core.Graph;
using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.UI.Controls
{
    public partial class GraphNode : ComponentBase
    {
        [Parameter]
        public Core.Graph.IDevGraphDefinition DevGraphDefinition { get; set; }

        [Parameter]
        public Core.Graph.IDevGraphNode DevGraphNode { get; set; }

        [Parameter]
        public GraphArea GraphArea { get; set; }

        private GraphNodeSavedContent? GraphNodeSavedContent { get; set; }


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                GraphArea.Nodes[DevGraphNode] = this;

                if (DevGraphNode.AdditionalContent.TryGetValue("GraphNodeUI", out var content))
                    GraphNodeSavedContent = System.Text.Json.JsonSerializer.Deserialize<GraphNodeSavedContent>(content);
                else
                    GraphNodeSavedContent = new GraphNodeSavedContent();

                DevGraphNode.AdditionalContentProvider["GraphNodeUI"] = () => System.Text.Json.JsonSerializer.Serialize(GraphNodeSavedContent);

                StateHasChanged();
            }
        }

        #region GetParameterAbsolutePosition

        public System.Drawing.PointF? GetParameterAbsolutePosition(IDevGraphNodeParameter devGraphNodeParameter)
        {
            if (GraphNodeSavedContent == null)
                return null;

            var index = devGraphNodeParameter.IsInput ? GetIndex(DevGraphNode.Inputs, devGraphNodeParameter) : GetIndex(DevGraphNode.Outputs, devGraphNodeParameter);

            if (index == null)
                return null;

            float y = GraphNodeSavedContent.Location.Y + 38 + index.Value * 24;
            float x = GraphNodeSavedContent.Location.X + (devGraphNodeParameter.IsInput ? 1 : GetWidth() - 1);

            return new System.Drawing.PointF(x, y);
        }

        private int? GetIndex(ICollection<IDevGraphNodeParameter> inputs, IDevGraphNodeParameter devGraphNodeParameter)
        {
            int i = 0;
            foreach (var input in inputs)
            {
                if (input == devGraphNodeParameter)
                    return i;

                ++i;
            }
            return null;
        }

        #endregion

        #region GetWidth

        private float GetWidth()
        {
            return Math.Max(100, DevGraphNode.Name.Length * 5 + 20); // basic formulas to define the width of the node based on the name of the node
        }

        #endregion

        #region Dragging

        private double DragStartX;
        private double DragStartY;

        System.Drawing.PointF InitialDragNodeLocation;

        private void OnDragStart(DragEventArgs args)
        {
            if (GraphNodeSavedContent == null)
                throw new Exception("GraphNodeSavedContent should be set");

            DragStartX = args.ClientX;
            DragStartY = args.ClientY;

            InitialDragNodeLocation = GraphNodeSavedContent.Location;
        }

        private void OnDrag(DragEventArgs args)
        {
            if (GraphNodeSavedContent == null)
                throw new Exception("GraphNodeSavedContent should be set");

            GraphNodeSavedContent.Location = new System.Drawing.PointF((float)(InitialDragNodeLocation.X + args.ClientX - DragStartX), (float)(InitialDragNodeLocation.Y + args.ClientY - DragStartY));

            GraphArea.GraphNodeMoved();
        }
        private void OnDragEnd(DragEventArgs args)
        {
            OnDrag(args);

            GraphArea.GraphNodeMoved();
        }

        #endregion
    }
}