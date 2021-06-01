using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphExitPoint : DevGraphNode, IDevGraphEntryPoint
    {
        public DevGraphExitPoint(IEnumerable<IDevGraphNodeParameter> inputs)
        {
            Inputs = inputs.ToList();
            Outputs = new List<IDevGraphNodeParameter>();
        }

        public override ICollection<IDevGraphNodeParameter> Inputs { get; }

        public override ICollection<IDevGraphNodeParameter> Outputs { get; }
    }
}
