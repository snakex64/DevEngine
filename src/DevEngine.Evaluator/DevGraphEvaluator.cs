using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
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

            var entryPointInstance = new DevGraphNodeInstance(devMethod.GraphDefinition.EntryPoint);

            foreach (var parameter in inputs)
            {
                var nodeParameter = entryPointInstance.GraphNode.Outputs.FirstOrDefault(x => x.Name == parameter.Key) ?? throw new Exception("Input not found:" + parameter.Key);
                entryPointInstance.Parameters[nodeParameter] = parameter.Value;
            }

            var exitNodeInstance = EvaluateFromEntryToExit(graphInstance, entryPointInstance);

            outputs = exitNodeInstance.Parameters.Where(x => x.Key.IsInput).ToDictionary(x => x.Key.Name, x => x.Value);
        }

        private DevGraphNodeInstance EvaluateFromEntryToExit(DevGraphInstance devGraphInstance, DevGraphNodeInstance entryNodeInstance)
        {
            while (true)
            {



            }
        }


    }
}
