using DevEngine.Core.Graph;
using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevEngine.UI.Controls
{
    public partial class GraphNode : ComponentBase
    {
        [Parameter]
        public IDevGraphDefinition DevGraphDefinition { get; set; }

        [Parameter]
        public IDevGraphNode DevGraphNode { get; set; }

        [Parameter]
        public GraphArea GraphArea { get; set; }

        public GraphNodeSavedContent? GraphNodeSavedContent { get; private set; }


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

                DevGraphNode.AdditionalContentToBeSerialized["GraphNodeUI"] = GraphNodeSavedContent;

                StateHasChanged();
            }
        }

        #region GetParameterAbsolutePosition

        private float? GetAbsoluteYPositionFromParameterIndex(int index)
        {
            return GraphNodeSavedContent?.Location.Y + 39 + index * 24;
        }

        public System.Drawing.PointF? GetParameterAbsolutePosition(IDevGraphNodeParameter devGraphNodeParameter)
        {
            if (GraphNodeSavedContent == null)
                return null;

            var index = devGraphNodeParameter.IsInput ? GetIndex(DevGraphNode.Inputs, devGraphNodeParameter) : GetIndex(DevGraphNode.Outputs, devGraphNodeParameter);

            if (index == null)
                return null;

            float y = GetAbsoluteYPositionFromParameterIndex(index.Value) ?? throw new Exception("Unable to get y position of parameter node");
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

        #region GetNodeRectangle

        public System.Drawing.RectangleF? GetNodeRectangle()
        {
            if (GraphNodeSavedContent == null)
                return null;

            var lastParameterIndex = Math.Max(DevGraphNode.Inputs.Count, DevGraphNode.Outputs.Count);
            return new System.Drawing.RectangleF(GraphNodeSavedContent.Location, new System.Drawing.SizeF(GetWidth(), GetAbsoluteYPositionFromParameterIndex(lastParameterIndex) ?? throw new Exception("Unable to get node rectangle")));
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

        #region Dragging parameter

        public void OnNodeParameterDragStart(GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            GraphArea.OnNodeParameterDragStart(this, graphNodeParameter, args);
        }
        public void OnNodeParameterDrag(GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            GraphArea.OnNodeParameterDrag(this, graphNodeParameter, args);
        }
        public void OnNodeParameterDragEnd(GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            GraphArea.OnNodeParameterDragEnd(this, graphNodeParameter, args);
        }


        #endregion
    }
}