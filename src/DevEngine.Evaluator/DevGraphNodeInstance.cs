using DevEngine.Core.Graph;
using DevEngine.Evaluator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    public class DevGraphNodeInstance: IDevGraphNodeInstance
    {
        public IDevGraphNode GraphNode { get; }

        public IDictionary<IDevGraphNodeParameter, DevObject> Parameters { get; } = new Dictionary<IDevGraphNodeParameter, DevObject>();

        public IDevGraphInstance DevGraphInstance { get; }

        public DevGraphNodeInstance(IDevGraphNode graphNode, IDevGraphInstance devGraphInstance)
        {
            GraphNode = graphNode;
            DevGraphInstance = devGraphInstance;

            devGraphInstance.NodeInstances[graphNode] = this;
        }
    }
}
