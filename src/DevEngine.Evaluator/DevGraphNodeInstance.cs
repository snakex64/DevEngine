using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    public class DevGraphNodeInstance
    {
        public readonly IDevGraphNode GraphNode;

        public Dictionary<IDevGraphNodeParameter, DevObject> Parameters { get; } = new();

        public DevGraphNodeInstance(IDevGraphNode graphNode)
        {
            GraphNode = graphNode;
        }
    }
}
