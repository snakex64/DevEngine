using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent getting a parameter passed to the method
    /// IsExecNode should always be false
    /// There should only be no input nodes
    /// There should only be one output node, the value
    /// </summary>
    public interface IDevGraphInputParameterGet : IDevGraphNode
    {
        IDevGraphNodeParameter ParameterToGet { get; }
    }
}
