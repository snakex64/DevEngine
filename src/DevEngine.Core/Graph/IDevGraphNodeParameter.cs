using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNodeParameter
    {
        IDevGraphNode ParentNode { get; }

        bool IsInput { get; }

        bool IsOutput => !IsInput;

        IDevType Type { get; }

        string Name { get; }

        ICollection<IDevGraphNodeParameter> Connections { get; }
    }
}
