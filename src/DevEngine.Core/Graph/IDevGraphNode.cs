using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNode
    {
        string Name { get; set; }

        /// <summary>
        /// If true, the node contains one ExecType as the first input, it's not inlined and will follow the flow of execution
        /// It doesn't mean that it contains an ExecType output
        /// </summary>
        bool IsExecNode { get; }

        ICollection<IDevGraphNodeParameter> Inputs { get; }

        ICollection<IDevGraphNodeParameter> Outputs { get; }

    }
}
