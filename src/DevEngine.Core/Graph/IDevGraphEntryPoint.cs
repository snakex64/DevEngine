using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent the entry point of a graph, example in a method graph this will contain the exec node and the input parameters
    /// </summary>
    public interface IDevGraphEntryPoint: IDevGraphNode
    {
        IDevGraphNodeParameter ExecNodeParameter => Outputs.Single(x => x.Type == DevExecType.ExecType);
    }
}
