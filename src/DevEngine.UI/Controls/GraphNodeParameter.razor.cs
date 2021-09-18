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

        private void OnConstantValueChanged(string newValue)
        {
            DevGraphNodeParameter.ConstantValueStr = newValue;
        }
    }
}