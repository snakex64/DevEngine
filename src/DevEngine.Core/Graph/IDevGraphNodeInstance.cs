using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNodeInstance
    {
        IDevGraphInstance DevGraphInstance { get; }

        IDevGraphNode GraphNode { get; }

        IDictionary<IDevGraphNodeParameter, DevObject> Parameters { get; }
    }
}
