using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    internal class DevGraphInstance
    {
        public readonly IDevGraphDefinition GraphDefinition;

        public readonly DevObject Self;

        public readonly Dictionary<string, DevObject> LocalVariables;

        public readonly Dictionary<IDevGraphNode, DevGraphNodeInstance> NodeInstances = new();

        public DevGraphInstance(IDevGraphDefinition graphDefinition, DevObject self)
        {
            GraphDefinition = graphDefinition;
            Self = self;
            LocalVariables = new Dictionary<string, DevObject>();
        }
    }
}
