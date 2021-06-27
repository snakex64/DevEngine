using DevEngine.Core.Graph;
using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace DevEngine.UI.Controls
{
    public partial class GraphArea : ComponentBase
    {
        [Parameter]
        public IDevGraphDefinition DevGraphDefinition { get; set; }

        private GraphSavedContent GraphSavedContent { get; set; } = new Shared.GraphSavedContent();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }


        private ElementReference? GraphAreaDivRef;


        public readonly IDictionary<IDevGraphNode, GraphNode> Nodes = new Dictionary<IDevGraphNode, GraphNode>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (DevGraphDefinition.AdditionalContent.TryGetValue("GraphArea", out var content))
                    GraphSavedContent = System.Text.Json.JsonSerializer.Deserialize<GraphSavedContent>(content) ?? new GraphSavedContent();
                else
                    GraphSavedContent = new GraphSavedContent();

                DevGraphDefinition.AdditionalContentToBeSerialized["GraphArea"] = GraphSavedContent;
            }

            if (GraphAreaObjectReference == null && GraphAreaDivRef != null)
            {
                await InitializeClientSide();
                StateHasChanged();
            }
        }

        #region GetParameterAbsolutePosition

        private System.Drawing.PointF? GetParameterAbsolutePosition(IDevGraphNodeParameter devGraphNodeParameter)
        {
            if (!Nodes.TryGetValue(devGraphNodeParameter.ParentNode, out var node))
                return null;

            return node.GetParameterAbsolutePosition(devGraphNodeParameter);
        }

        #endregion

        #region GetSvgPath

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

        #endregion

        #region GraphNodeMoved

        public void GraphNodeMoved()
        {
            StateHasChanged();
        }

        #endregion

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

            if (changeState)
                StateHasChanged();
        }

        #endregion

        #region Initialize Client side

        private DotNetObjectReference<GraphArea>? GraphAreaObjectReference;

        private ValueTask InitializeClientSide()
        {
            GraphAreaObjectReference = DotNetObjectReference.Create(this);

            return JSRuntime.InvokeVoidAsync("initializeGraphAreaClientSide", GraphAreaObjectReference, GraphAreaDivRef ?? throw new Exception("Cannot initialize client side with null GraphAreaDivRef"));
        }

        #endregion

        #region BackgroundMovedFromClient

        private uint SmallMovementIncrement = 0;
        [JSInvokable]
        public bool BackgroundMovedFromClient(float newX, float newY, float newSizeX, float newSizeY)
        {
            if (newX > 0 || newY > 0)
            {
                // we can't drag the background to the right, it would means, to be able to drag nodes to the left we'd have to "leave" the background, which we can't do visually
                // to patch that, we drag the background back to zero, but we move all the nodes right by the same amount.

                foreach (var node in Nodes)
                {
                    var content = node.Value.GraphNodeSavedContent;
                    if (content != null)
                        content.Location = new PointF(newX > 0 ? content.Location.X + newX : content.Location.X, newY > 0 ? content.Location.Y + newY : content.Location.Y);
                }

                // ugly patch so that blazor understand that we changed the X,Y location
                // if not, he will go : "oh it was 0 and it's still 0, nice nothing to do", not understanding the value was changed client side
                unchecked
                {
                    if (newX > 0)
                        newX = float.Parse($"0.{++SmallMovementIncrement}");
                    if (newY > 0)
                        newY = float.Parse($"0.{++SmallMovementIncrement}");
                }
            }

            GraphSavedContent.BackgroundPosition = new PointF(newX, newY);
            GraphSavedContent.AreaSize = new SizeF(newSizeX, newSizeY);

            StateHasChanged();

            return true;
        }

        #endregion
    }
}