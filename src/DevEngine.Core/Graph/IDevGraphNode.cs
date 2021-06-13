using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public enum DevGraphNodeExecuteResult
    {
        Continue, Break, Exit
    }
    public interface IDevGraphNode
    {
        string Name { get; }

        /// <summary>
        /// If true, the node contains one ExecType as the first input, it's not inlined and will follow the flow of execution
        /// It doesn't mean that it contains an ExecType output
        /// </summary>
        bool IsExecNode { get; }

        ICollection<IDevGraphNodeParameter> Inputs { get; }

        ICollection<IDevGraphNodeParameter> Outputs { get; }


        /// <summary>
        /// Execute the node
        /// </summary>
        DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance);

        /// <summary>
        /// Returns true if, when the output exec is done executing, the graph must come back to this node instead of leaving
        /// This is generally false for all nodes, except nodes like "while" or "sequence", who needs to execute multiple exec paths
        /// </summary>
        bool ExecuteExecAsSubGraph { get; }

        /// <summary>
        /// Return the exec node to go through. This will be called multiple times only if <seealso cref="ExecuteExecAsSubGraph"/> is true
        /// Not called if the IsExecNode == false
        /// Can return null if there's no output exec
        /// </summary>
        IDevGraphNodeParameter? GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance);


        /// <summary>
        /// Additional content to be saved and loaded with this node.
        /// One should only add one KeyValuePair per system using this node, ex a single one for the XY position, do not over-use this
        /// </summary>
        IDictionary<string, string> AdditionalContent { get; }

        /// <summary>
        /// Each provider is called before this node is saved, and the content is added to <seealso cref="AdditionalContent"/>.
        /// if null is returned, the content is not added, any previous value saved with this key will be removed
        /// </summary>
        IDictionary<string, Func<string?>> AdditionalContentProvider { get; }
    }
}
