using DevEngine.Core.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent a node calling a method in the graph
    /// IDevGraphNode.IsExecNode should always be 'true' for this, since there should always be a ExecType input for the first input
    /// </summary>
    public interface IDevGraphMethodCall: IDevGraphNode
    {
        IDevMethod Method { get; }
    }
}
