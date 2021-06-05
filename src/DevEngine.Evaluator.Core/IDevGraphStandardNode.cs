using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator.Core
{
    /// <summary>
    /// Represent a node that can be both evaluated byt DevEngine.Evaluator, and compiled by the future DevEngine.Compiler
    /// </summary>
    public interface IDevGraphStandardNode : IDevGraphNode
    {

        /// <summary>
        /// Execute the node
        /// </summary>
        void Execute(IDevGraphNodeInstance devGraphNodeInstance);

        /// <summary>
        /// Returns true if, when the output exec is done executing, the graph must come back to this node instead of leaving
        /// This is generally false for all nodes, except nodes like "while" or "sequence", who needs to execute multiple exec paths
        /// </summary>
        bool ExecuteExecAsSubGraph { get; }

        /// <summary>
        /// Return the exec node to go through. This will be called multiple times only if <seealso cref="ExecuteExecAsSubGraph"/> is true
        /// Not called if the IsExecNode == false
        /// </summary>
        IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance);
    }
}
