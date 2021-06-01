using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNodeParameter
    {
        bool IsInput { get; set; }

        bool IsOutput => !IsInput;

        IDevType Type { get; }

        string Name { get; }

        ICollection<IDevGraphNodeParameter> Connections { get; }
    }
}
