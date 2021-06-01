using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphEntryPoint : DevGraphNode, IDevGraphEntryPoint
    {
        public DevGraphEntryPoint(IEnumerable<IDevGraphNodeParameter> inputs, IEnumerable<IDevGraphNodeParameter> outputs)
        {
            Inputs = inputs.ToList();
            Outputs = outputs.ToList();
        }

        public override ICollection<IDevGraphNodeParameter> Inputs { get; }

        public override ICollection<IDevGraphNodeParameter> Outputs { get; }
    }
}
