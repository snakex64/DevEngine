using DevEngine.Core.Graph;
using DevEngine.Evaluator.Core;
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
        private bool EvaluateUntilExit(DevGraphInstance devGraphInstance, DevGraphNodeInstance startNode, [MaybeNullWhen(false)] out DevGraphNodeInstance exitInstance)
        {
            if (!startNode.GraphNode.IsExecNode)
                throw new Exception("Cannot use EvaluateUntilExit on a node with no exec parameter");


            DevGraphNodeInstance currentNode = startNode;

            var stack = new Stack<DevGraphNodeInstance>();

            while (true)
            {
                switch(currentNode.GraphNode)
                {
                    case IDevGraphStandardNode standardNode:

                        break;
                }


            }
        }

//        private void PopulateNode


    }
}
