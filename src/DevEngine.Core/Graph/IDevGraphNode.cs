using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DevEngine.Core.Graph
{
    public enum DevGraphNodeExecuteResult
    {
        Continue, Break, Exit
    }
    public interface IDevGraphNode
    {
        Guid Id { get; }

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
        IDictionary<string, string> AdditionalContent { get; set; }

        /// <summary>
        /// Content to be saved with this graph node
        /// Every Value will be serialized and saved to <seealso cref="AdditionalContent"/>
        /// If a Value is null, any previously saved content with that Key will be removed
        /// If a Key existed before and isn't defined now, it wil stay the same
        /// </summary>
        IDictionary<string, object?> AdditionalContentToBeSerialized { get; set; }

        void InitializeAfterPreLoad()
        {
            if (!AdditionalContent.TryGetValue("ConstantInputs", out var constantsStr))
                return;

            var constants = JsonSerializer.Deserialize<Dictionary<string, string>>(constantsStr);
            if (constants == null)
                return;
            // load the constant values
            foreach (var input in Inputs)
            {
                if (input.Type.IsBasicType)
                {
                    if (constants.TryGetValue(input.Name, out var constant))
                        input.ConstantValueStr = constant;
                }
            }
        }

        void OnParameterConnectionChanged(IDevGraphNodeParameter parameter)
        { }

    }
}
