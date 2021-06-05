using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator.Core
{
    public interface IDevGraphInstance
    {
        IDevGraphDefinition GraphDefinition { get; }

        DevObject Self { get; }

        Dictionary<string, DevObject> LocalVariables { get; }

        Dictionary<IDevGraphNode, IDevGraphNodeInstance> NodeInstances { get; }
    }
}
