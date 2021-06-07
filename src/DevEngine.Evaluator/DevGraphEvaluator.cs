using DevEngine.Core;
using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    public class DevGraphEvaluator
    {
        public void Evaluate(DevObject self, FakeTypes.Method.DevMethod devMethod, Dictionary<string, DevObject> inputs, out Dictionary<string, DevObject> outputs)
        {
            if (devMethod.GraphDefinition == null)
                throw new Exception("Cannot evaluate method, no graph provided");

            var graphInstance = new DevGraphInstance(devMethod.GraphDefinition, self);

            var entryPointInstance = new DevGraphNodeInstance(devMethod.GraphDefinition.EntryPoint, graphInstance);

            foreach (var parameter in inputs)
            {
                var nodeParameter = entryPointInstance.GraphNode.Outputs.FirstOrDefault(x => x.Name == parameter.Key) ?? throw new Exception("Input not found:" + parameter.Key);
                entryPointInstance.Parameters[nodeParameter] = parameter.Value;
            }

            var exitedCorrectly = EvaluateUntilExit(graphInstance, entryPointInstance, out var exitNodeInstance);
            if (!exitedCorrectly || exitNodeInstance == null)
                outputs = new Dictionary<string, DevObject>();
            else
                outputs = exitNodeInstance.Parameters.Where(x => x.Key.IsInput).ToDictionary(x => x.Key.Name, x => x.Value);
        }

        /// <summary>
        /// Evaluate down the path until it either hit the end of the path, or an exit node
        /// </summary>
        /// <param name="exitInstance">The node instance of the exit node, if any</param>
        /// <returns>True if it ended with an exit not</returns>
        private bool EvaluateUntilExit(DevGraphInstance devGraphInstance, DevGraphNodeInstance startNode, [MaybeNullWhen(false)] out IDevGraphNodeInstance exitInstance)
        {
            if (!startNode.GraphNode.IsExecNode)
                throw new Exception("Cannot use EvaluateUntilExit on a node with no exec parameter");

            var nextExecutionNode = startNode.GraphNode.GetNextExecutionParameter(startNode);

            // shouldn't happen ?
            if (nextExecutionNode == null)
            {
                exitInstance = null;
                return false;
            }

            var currentNode = devGraphInstance.NodeInstances[nextExecutionNode.Connections.First().ParentNode];

            var stack = new Stack<IDevGraphNodeInstance>();

            while (true)
            {
                if (currentNode.GraphNode.ExecuteExecAsSubGraph)
                    stack.Push(currentNode);

                PopulateNodeInputs(currentNode);

                var executeResult = currentNode.GraphNode.Execute(currentNode);

                if (executeResult == DevGraphNodeExecuteResult.Exit)
                {
                    // if it was an exit node, there's nothing to do, the PopulateNodeInputs should already have filled the values to return
                    exitInstance = currentNode;
                    return true;
                }

                var nextExec = currentNode.GraphNode.GetNextExecutionParameter(currentNode);

                // if there's nothing else to exec, just pop the stack and roll back to the upper node
                if (nextExec == null)
                {
                    // if the stack is empty, we just quit, this is an error from the user who created this graph, this shouldn't have happened but we still gracefully exit
                    if (stack.Count == 0)
                    {
                        exitInstance = null;
                        return false;
                    }

                    currentNode = stack.Pop();
                }
                else
                    currentNode = devGraphInstance.NodeInstances[nextExec.ParentNode];
            }
        }

        private void PopulateNodeInputs(IDevGraphNodeInstance node)
        {
            foreach (var input in node.GraphNode.Inputs)
            {
                var otherNodeParameter = input.Connections.FirstOrDefault();

                // not plugged to anything
                if (otherNodeParameter == null)
                    node.Parameters[input] = new DevObject(input.Type, null);
                else if(input.Type != DevExecType.ExecType) // we don't need to connect execs
                {
                    // get the linked node and populate it's own inputs
                    var otherNode = node.DevGraphInstance.NodeInstances[otherNodeParameter.ParentNode];
                    PopulateNodeInputs(otherNode);

                    // then set the newly set value from the other node, to ourself
                    node.Parameters[input] = otherNode.Parameters[otherNodeParameter];
                }
            }

            if (!node.GraphNode.IsExecNode) // we only execute non-exec nodes, like "Add"
            {
                var nextExec = node.GraphNode.Execute(node);
                if (nextExec != DevGraphNodeExecuteResult.Continue)
                    throw new Exception("Can't break or exit in a non-exec nodes");
            }
        }


    }
}
