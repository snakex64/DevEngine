using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.UI.Controls
{
    public partial class GraphNodeParameter : ComponentBase
    {
        [EditorRequired]
        [Parameter]
        public Core.Graph.IDevGraphNodeParameter DevGraphNodeParameter { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public GraphNode ParentGraphNode { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public GraphArea GraphArea { get; set; } = null!;


        [CascadingParameter]
        public RightClickController RightClickController { get; set; } = null!;

        public bool IsGenericType
        {
            get
            {
                if (DevGraphNodeParameter.Type is RealTypes.Class.RealClass real)
                    return real.IsUnknownedType;

                return false;
            }
        }

        public bool IsBasicType
        {
            get
            {
                if (IsGenericType)
                    return false;

                if (DevGraphNodeParameter.Type is RealTypes.Class.RealClass real)
                    return real.IsBasicType;

                return false;
            }
        }

        public string? ConstantValueStr
        {
            get
            {
                if (DevGraphNodeParameter.IsOutput)
                    throw new Exception("Cannot get constant value for an output");

                return DevGraphNodeParameter.ConstantValueStr;
            }
        }

        #region Drag

        private void OnDragStart(DragEventArgs args)
        {
            ParentGraphNode.OnNodeParameterDragStart(this, args);
        }
        private void OnDrag(DragEventArgs args)
        {
            ParentGraphNode.OnNodeParameterDrag(this, args);
        }
        private void OnDragEnd(DragEventArgs args)
        {
            ParentGraphNode.OnNodeParameterDragEnd(this, args);
        }

        #endregion

        #region OnConstantValueChanged

        private void OnConstantValueChanged(string newValue)
        {
            DevGraphNodeParameter.ConstantValueStr = newValue;
        }

        #endregion

        #region GetToolTip

        public string GetToolTip()
        {
            if (IsGenericType)
                return "<T>";

            if (DevGraphNodeParameter.Type is RealTypes.Class.RealClass real)
            {
                if (real.IsBasicType)
                {
                    if (real.RealType == typeof(int))
                        return "int";
                    else if (real.RealType == typeof(float))
                        return "float";
                    else if (real.RealType == typeof(double))
                        return "double";
                    else if (real.RealType == typeof(string))
                        return "string";
                    else if (real.RealType == typeof(bool))
                        return "boolean";
                    else if (real.RealType == typeof(char))
                        return "char";
                    else if (real.RealType == typeof(long))
                        return "long";
                    else
                        return real.RealType.Name;
                }
                else
                {
                    return real.RealType.Name;
                }
            }
            else
                return DevGraphNodeParameter.Type.TypeNamespaceAndName;
        }

        #endregion

        #region OnBackgroundClicked

        private void OnRightClick(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button != 2)
                return;


            RightClickController.DisplayRightClickMenu(mouseEventArgs, new List<RightClickController.MenuItem>
            {
                new RightClickController.MenuItem("Disconnect all", ClearConnection)
            });
        }

        #endregion

        #region ClearConnections

        private void ClearConnection()
        {
            foreach (var connection in DevGraphNodeParameter.Connections.ToList())
                ParentGraphNode.DevGraphDefinition.DisconnectNodesParameters(DevGraphNodeParameter, connection);

            GraphArea.ForceStateHasChanged();
        }

        #endregion
    }
}