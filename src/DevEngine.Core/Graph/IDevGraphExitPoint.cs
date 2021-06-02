using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent the exist point of a graph.
    /// There can be multiple exit point, just like you can "return" in multiple places in a method
    /// An exit point contains an ouput parameter for the exec, one for each "out" parameter and an additional output for the return value of the method itself
    /// </summary>
    public interface IDevGraphExitPoint: IDevGraphNode
    {
        IDevGraphNodeParameter ExecNodeParameter => Inputs.Single(x => x.Type == DevExecType.ExecType);

        IDevGraphNodeParameter ReturnNodeParameter => Inputs.Single(x => x.Name == "return");
    }
}
