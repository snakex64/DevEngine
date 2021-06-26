using DevEngine.Core.Graph;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        #region node parameter dragging


        private GraphNodeParameter? CurrentDraggedNodeParameter;
        private PointF CurrentNodeParameterDragPosition;
        private PointF CurrentNodeParameterDragStartPosition;
        internal void OnNodeParameterDragStart(GraphNode graphNode, GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            CurrentDraggedNodeParameter = graphNodeParameter;
            CurrentNodeParameterDragStartPosition = CurrentNodeParameterDragPosition = new PointF((float)args.ClientX, (float)args.ClientY);
        }
        internal void OnNodeParameterDrag(GraphNode graphNode, GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            CurrentNodeParameterDragPosition = new PointF((float)args.ClientX, (float)args.ClientY);
            StateHasChanged();
        }

        internal void OnNodeParameterDragEnd(GraphNode graphNode, GraphNodeParameter graphNodeParameter, DragEventArgs args)
        {
            if (CurrentDraggedNodeParameter == null)
            {
                CurrentDraggedNodeParameter = null;
                StateHasChanged();
                return;
            }

            // search the node under the mouse
            var initialPos = GetParameterAbsolutePosition(CurrentDraggedNodeParameter.DevGraphNodeParameter);
            if (initialPos == null)
            {
                CurrentDraggedNodeParameter = null;
                StateHasChanged();
                return;
            }
            var newPos = new PointF(initialPos.Value.X + CurrentNodeParameterDragPosition.X - CurrentNodeParameterDragStartPosition.X, initialPos.Value.Y + CurrentNodeParameterDragPosition.Y - CurrentNodeParameterDragStartPosition.Y);

            // if we're an input, we try to connect to outputs, vice versa
            var nodeParameterFound = GetNodeParameterUnderMouse(newPos, graphNodeParameter.DevGraphNodeParameter.IsOutput);


            if (nodeParameterFound != null)
                ConnectNodesParameters(nodeParameterFound, CurrentDraggedNodeParameter.DevGraphNodeParameter, false);

            CurrentDraggedNodeParameter = null;
            StateHasChanged();
        }

        #endregion

        #region EnumerateNodesUnderMouse

        private IEnumerable<KeyValuePair<IDevGraphNode, GraphNode>> EnumerateNodesUnderMouse(PointF mouse)
        {
            foreach (var node in Nodes)
            {
                var position = node.Value.GetNodeRectangle();
                if (position == null)
                    continue;

                // if the box of the node is under the mouse, check the parameters to find the one under the mouse
                if (position.Value.Contains(mouse))
                {
                    yield return node;
                }
            }
        }

        #endregion

        #region GetNodeParameterUnderMouse

        private IDevGraphNodeParameter? GetNodeParameterUnderMouse(PointF mouse, bool lookAtInputs)
        {
            foreach (var node in EnumerateNodesUnderMouse(mouse))
            {
                var parameters = lookAtInputs ? node.Key.Inputs : node.Key.Outputs;
                foreach (var parameter in parameters)
                {
                    var parameterPosition = node.Value.GetParameterAbsolutePosition(parameter);
                    if (parameterPosition == null)
                        continue;
                    if (Math.Abs(parameterPosition.Value.Y - mouse.Y) < 10)
                    {
                        // we found it, wouhou
                        return parameter;
                    }
                }
            }
            return null;
        }

        #endregion

        #region ConnectNodesParameters

        private bool ConnectNodesParameters(IDevGraphNodeParameter parameter1, IDevGraphNodeParameter parameter2, bool changeState)
        {
            // can't connect input to input or output to output
            if (parameter1.IsInput == parameter2.IsInput)
                return false;


            var inputParameter = parameter1.IsInput ? parameter1 : parameter2;
            var outputParameter = parameter1.IsInput ? parameter2 : parameter1;

            if (!outputParameter.Type.CanBeAssignedTo(inputParameter.Type))
                return false;

            // exec type is the only type that allows plugging multiple values in an input
            if (inputParameter.Type != Core.DevExecType.ExecType)
            {
                // if not, we clear the connections to the input before plugging something else
                foreach (var connection in inputParameter.Connections.ToList()) // ToList() is there to be able to modify the Connections while inside the foreach
                    RemoveNodeParameterConnection(inputParameter, connection, false);
            }

            DevGraphDefinition.ConnectNodesParameters(outputParameter, inputParameter);

            if (changeState)
                StateHasChanged();

            return true;
        }

        #endregion

        #region RemoveNodeParameterConnection

        private void RemoveNodeParameterConnection(IDevGraphNodeParameter parameter1, IDevGraphNodeParameter parameter2, bool changeState = true)
        {
            parameter1.Connections.Remove(parameter2);
            parameter2.Connections.Remove(parameter1);

            if(changeState)
                StateHasChanged();
        }

        #endregion
    }
}