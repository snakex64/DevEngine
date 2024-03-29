﻿using DevEngine.Core.Graph;
using DevEngine.UI.Nodes;
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
        [EditorRequired]
        [Parameter]
        public IDevGraphDefinition DevGraphDefinition { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public IDevGraphNode DevGraphNode { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public GraphArea GraphArea { get; set; } = null!;

        [CascadingParameter]
        public RightClickController RightClickController { get; set; } = null!;

        public GraphNodeSavedContent? GraphNodeSavedContent { get; private set; }


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                GraphArea.Nodes[DevGraphNode] = this;

                if (DevGraphNode.AdditionalContentToBeSerialized.TryGetValue("GraphNodeUI", out var graphNodeUI) && graphNodeUI != null)
                    GraphNodeSavedContent = (GraphNodeSavedContent)graphNodeUI;
                else if (DevGraphNode.AdditionalContent.TryGetValue("GraphNodeUI", out var content))
                    GraphNodeSavedContent = System.Text.Json.JsonSerializer.Deserialize<GraphNodeSavedContent>(content);
                else
                    GraphNodeSavedContent = new GraphNodeSavedContent();

                DevGraphNode.AdditionalContentToBeSerialized["GraphNodeUI"] = GraphNodeSavedContent;

                if (DevGraphNode.AdditionalContent.ContainsKey("GenericNodeType") && DevGraphNode is not DevGenericNodeBuilder)
                {
                    DevGraphNode.AdditionalContent.Remove("GenericNodeType");
                    DevGraphNode.AdditionalContentToBeSerialized.Remove("GenericNodeType");

                    GraphArea.ForceStateHasChanged();
                }
                else
                    StateHasChanged();
            }
        }

        #region OnRightClick

        private void OnRightClick(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button != 2)
                return;


            RightClickController.DisplayRightClickMenu(mouseEventArgs, new List<RightClickController.MenuItem>
            {
                new RightClickController.MenuItem("Remove", RemoveNode)
            });
        }

        #endregion

        #region RemoveNode

        private void RemoveNode()
        {
            GraphArea.RemoveNode(DevGraphNode);

            GraphArea.ForceStateHasChanged();
        }

        #endregion

        #region GetParameterAbsolutePosition

        private float? GetAbsoluteYPositionFromParameterIndex(int index)
        {
            return GraphNodeSavedContent?.Location.Y + 39 + index * 24 + (DevGraphNode.AmountOfDifferentVersions > 0 ? 50 : 0);
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
            var parameterMaxLength = DevGraphNode.Inputs.Concat(DevGraphNode.Outputs).Select(x => x.Name.Length + (x.IsOutput || x.Type.IsUnknownedType || x.Connections.Any() ? 0 : 10)).DefaultIfEmpty(5).Max();

            return Math.Max(100, Math.Max(DevGraphNode.Name.Length, parameterMaxLength) * 8 + 20); // basic formulas to define the width of the node based on the name of the node
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

        #region Change version

        private void DecreaseVersion()
        {
            var c = DevGraphNode.Version;
            if (c > 1)
                ChangeVersion(c - 1);
            else if (c == 1 && DevGraphNode.AmountOfDifferentVersions != 1)
                ChangeVersion(DevGraphNode.AmountOfDifferentVersions);
        }

        private void IncreaseVersion()
        {
            var c = DevGraphNode.Version;
            if (c < DevGraphNode.AmountOfDifferentVersions)
                ChangeVersion(c + 1);
            else if (c == DevGraphNode.AmountOfDifferentVersions && c != 1)
                ChangeVersion(1);
        }


        private void ChangeVersion(int newVersion)
        {
            // backup the inputs and outputs, so we can update the graph definition
            var previousInputs = DevGraphNode.Inputs.ToList();
            var previousOutputs = DevGraphNode.Outputs.ToList();

            DevGraphNode.Version = newVersion;

            foreach (var previousParameter in previousInputs.Concat(previousOutputs))
            {
                if (DevGraphNode.Inputs.Contains(previousParameter) || DevGraphNode.Outputs.Contains(previousParameter))
                    continue; // nothing to do if the same parameter is still there

                if (!previousParameter.Connections.Any())
                    continue; // nothing to do if nothing was connected to it anyway

                var connectionsToRemake = previousParameter.Connections.ToList();
                // make sure they other side is disconnected
                foreach (var connection in connectionsToRemake)
                    DevGraphDefinition.DisconnectNodesParameters(previousParameter, connection);

                // remake the connections to the new parameter
                foreach (var connection in connectionsToRemake)
                {
                    var newParameter = (previousParameter.IsInput ? DevGraphNode.Inputs : DevGraphNode.Outputs).FirstOrDefault(x => x.Name == previousParameter.Name);

                    if (newParameter == null)
                        continue; // the parameter doesn't exist anymore

                    DevGraphDefinition.ConnectNodesParameters(newParameter, connection);
                }
            }

            GraphArea.ForceStateHasChanged();
        }
        #endregion
    }
}