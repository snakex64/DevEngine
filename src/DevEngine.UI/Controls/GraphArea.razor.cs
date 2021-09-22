using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using DevEngine.UI.Nodes;
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

        [Parameter]
        public IDevMethod? DevMethod { get; set; }

        private GraphSavedContent GraphSavedContent { get; set; } = new Shared.GraphSavedContent();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        public RightClickController RightClickController { get; set; }


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

        #region ForceStateHasChanged

        internal void ForceStateHasChanged()
        {
            StateHasChanged();
        }

        #endregion

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
            var nodeParameterFound = GetNodeParameterUnderMouse(newPos, graphNodeParameter.DevGraphNodeParameter.IsOutput, out var node);


            if (nodeParameterFound != null)
                ConnectNodesParameters(nodeParameterFound, CurrentDraggedNodeParameter.DevGraphNodeParameter, false);
            else if (node != null && !graphNodeParameter.DevGraphNodeParameter.Type.IsUnknownedType)
            {
                if (node.Value.Key is IDevGraphEntryPoint entry && graphNodeParameter.DevGraphNodeParameter.IsInput)
                {
                    // search for an available name
                    AddInputToGraph(graphNodeParameter, entry);
                }
                else if (node.Value.Key is IDevGraphExitPoint exit && graphNodeParameter.DevGraphNodeParameter.IsOutput)
                {
                    // search for an available name
                    AddOutputToGraph(graphNodeParameter, exit);
                }
            }

            CurrentDraggedNodeParameter = null;
            StateHasChanged();
        }


        #endregion

        #region AddInputToGraph

        private void AddInputToGraph(GraphNodeParameter graphNodeParameter, IDevGraphEntryPoint entry)
        {
            string name = graphNodeParameter.DevGraphNodeParameter.Name;
            for (int i = 1; entry.Outputs.Any(x => x.Name == name); ++i)
                name = $"{name}_{i}";

            if (DevMethod == null)
                throw new Exception("DevMethod can't be null here");

            DevMethod.AddInput(new FakeTypes.Method.DevMethodParameter(graphNodeParameter.DevGraphNodeParameter.Type, name, false, false));

            var newInput = entry.Outputs.First(x => x.Name == name); // get the new input, which is the output parameter node of the entry node... confusioooonnn

            DevGraphDefinition.ConnectNodesParameters(graphNodeParameter.DevGraphNodeParameter, newInput);

            // TODO --- must update opened tabs and all the other people calling that specific method, to make sure they visually have the new parameter!

        }

        #endregion

        #region AddOutputToGraph

        private void AddOutputToGraph(GraphNodeParameter parameterToConnect, IDevGraphExitPoint exit)
        {
            string name = parameterToConnect.DevGraphNodeParameter.Name;
            for (int i = 1; exit.Inputs.Any(x => x.Name == name); ++i)
                name = $"{name}_{i}";

            if (DevMethod == null)
                throw new Exception("DevMethod can't be null here");

            DevMethod.AddOutput(new FakeTypes.Method.DevMethodParameter(parameterToConnect.DevGraphNodeParameter.Type, name, true, false));

            var newOutput = exit.Inputs.First(x => x.Name == name); // get the new output, which is the input parameter node of the exit node... confusioooonnn

            DevGraphDefinition.ConnectNodesParameters(parameterToConnect.DevGraphNodeParameter, newOutput);

            // TODO --- must update opened tabs and all the other people calling that specific method, to make sure they visually have the new parameter!
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

        private IDevGraphNodeParameter? GetNodeParameterUnderMouse(PointF mouse, bool lookAtInputs, out KeyValuePair<IDevGraphNode, GraphNode>? nodeUnder)
        {
            nodeUnder = null;

            foreach (var node in EnumerateNodesUnderMouse(mouse))
            {
                nodeUnder = node;

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

            // trigger the parameter changed event for both the input and the output
            outputParameter.ParentNode.OnParameterConnectionChanged(outputParameter);
            inputParameter.ParentNode.OnParameterConnectionChanged(inputParameter);


            if (outputParameter.ParentNode is DevGenericNodeBuilder builder && builder.ReplaceGenericWithRealNode)
                ReplaceGenericBuilderWithRealNode(builder);
            if (inputParameter.ParentNode is DevGenericNodeBuilder builder2 && builder2.ReplaceGenericWithRealNode)
                ReplaceGenericBuilderWithRealNode(builder2);

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

        #region OnBackgroundClicked

        private void OnBackgroundClicked(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button != 2)
                return;

            void buildFragment(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<SearchContextualPopup>(0);

                builder.AddAttribute(0, "OnResultSelected", EventCallback.Factory.Create<DevGraphNodeSearchResult>(this, OnContextualSearchSelected));

                builder.CloseComponent();
            }

            RightClickController.DisplayRightClickMenu(mouseEventArgs, buildFragment);
        }

        #endregion

        #region OnContextualSearchSelected

        private void OnContextualSearchSelected(DevGraphNodeSearchResult result)
        {
            RightClickController.CloseRightClickMenu();

            IDevGraphNode node;

            // if it's a generic type, we have to put a generic builder instead, the user will be able to configure the type of the generic parameters with the builder
            if (result.GenericType != null)
            {
                node = new DevGenericNodeBuilder(Guid.NewGuid(), result.DisplayName, Program.Project, result.GenericType);

                DevGraphDefinition.AddNode(node);
            }
            else
            {
                node = result.BuildNewNode(Guid.NewGuid(), result.DisplayName, Program.Project);

                DevGraphDefinition.AddNode(node);
            }


            // doesn't work :(
            //node.AdditionalContentToBeSerialized["GraphNodeUI"] = new GraphNodeSavedContent()
            //{
            //    Location = new PointF((float)RightClickController.MenuX + GraphSavedContent.BackgroundPosition.X, (float)RightClickController.MenuY + GraphSavedContent.BackgroundPosition.Y)
            //};

            StateHasChanged();
        }

        #endregion

        #region RemoveNode

        public void RemoveNode(IDevGraphNode node)
        {
            DevGraphDefinition.RemoveNode(node);

            Nodes.Remove(node);
        }

        #endregion

        #region ReplaceGenericBuilderWithRealNode

        public void ReplaceGenericBuilderWithRealNode(DevGenericNodeBuilder devGenericNodeBuilder)
        {

            if (devGenericNodeBuilder.GenericNodeType == null)
                throw new Exception("devGenericNodeBuilder.GenericNodeType shouldn't be null here");

            if (devGenericNodeBuilder.Parameters == null)
                throw new Exception("devGenericNodeBuilder.Parameters shouldn't be null here");

            var generics = devGenericNodeBuilder.Parameters
                .Where(x => x.GenericName != null)
                .GroupBy(x => x.GenericName)
                .Select(x => ((RealTypes.Class.RealClass?)x.First().KnownedType)?.RealType ?? throw new Exception("Unable to get real type from generic"))
                .ToArray();

            var genericType = devGenericNodeBuilder.GenericNodeType.MakeGenericType(generics);

            var node = Standard.StandardSearchProvider.CreateNodeInstance(genericType, Guid.NewGuid(), devGenericNodeBuilder.Name, Program.Project);

            foreach (var genericParameter in devGenericNodeBuilder.Inputs.Concat(devGenericNodeBuilder.Outputs))
            {
                if (genericParameter.Connections.Count == 0)
                    continue;

                var nodeParameter = node.Inputs.Concat(node.Outputs).Single(x => x.IsInput == genericParameter.IsInput && x.Name == genericParameter.Name);


                foreach (var connection in genericParameter.Connections.ToList()) // the ToList is required since we're modifying the collection while in the foreach, which isn't allowed
                {
                    // remove the old connection
                    DevGraphDefinition.DisconnectNodesParameters(genericParameter, connection);

                    // add the new one
                    DevGraphDefinition.ConnectNodesParameters(nodeParameter, connection);
                }
            }

            node.AdditionalContent = devGenericNodeBuilder.AdditionalContent;
            node.AdditionalContentToBeSerialized = devGenericNodeBuilder.AdditionalContentToBeSerialized;

            DevGraphDefinition.AddNode(node);

            RemoveNode(devGenericNodeBuilder);

            StateHasChanged();
        }

        #endregion
    }
}