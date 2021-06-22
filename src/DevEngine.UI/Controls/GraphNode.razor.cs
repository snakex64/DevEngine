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

        private GraphNodeSavedContent? GraphNodeSavedContent { get; set; }


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                if (DevGraphNode.AdditionalContent.TryGetValue("GraphNodeUI", out var content))
                    GraphNodeSavedContent = System.Text.Json.JsonSerializer.Deserialize<GraphNodeSavedContent>(content);
                else
                    GraphNodeSavedContent = new GraphNodeSavedContent();

                DevGraphNode.AdditionalContentProvider["GraphNodeUI"] = () => System.Text.Json.JsonSerializer.Serialize(GraphNodeSavedContent);

                StateHasChanged();
            }
        }

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
        }
        private void OnDragEnd(DragEventArgs args)
        {
            OnDrag(args);
        }

        #endregion
    }
}