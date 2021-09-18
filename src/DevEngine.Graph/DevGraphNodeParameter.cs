using DevEngine.Core;
using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphNodeParameter : IDevGraphNodeParameter
    {
        public DevGraphNodeParameter(bool isInput, IDevType type, string name, IDevGraphNode parentNode)
        {
            IsInput = isInput;
            Type = type;
            Name = name;
            Connections = new List<IDevGraphNodeParameter>();
            ParentNode = parentNode;
        }

        public bool IsInput { get; }

        public IDevType Type { get; set; }

        public string Name { get; }

        public ICollection<IDevGraphNodeParameter> Connections { get; }

        public IDevGraphNode ParentNode { get; }

        public string? ConstantValueStr { get; set; }
    }
}
