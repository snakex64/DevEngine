using DevEngine.Core.Graph;
using DevEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    internal class DevGraphInstance : IDevGraphInstance
    {
        public IDevGraphDefinition GraphDefinition { get; }

        public DevObject Self { get; }

        public Dictionary<string, DevObject> LocalVariables { get; } = new();

        public Dictionary<IDevGraphNode, IDevGraphNodeInstance> NodeInstances { get; } = new();

        public DevGraphInstance(IDevGraphDefinition graphDefinition, DevObject self)
        {
            GraphDefinition = graphDefinition;
            Self = self;
        }
    }
}
