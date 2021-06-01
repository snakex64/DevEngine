using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphMethodCall : DevGraphNode, IDevGraphMethodCall
    {
        public DevGraphMethodCall(IDevMethod method, IEnumerable<IDevGraphNodeParameter> inputs, IEnumerable<IDevGraphNodeParameter> outputs)
        {
            Method = method;
            Inputs = inputs.ToList();
            Outputs = outputs.ToList();
        }

        public IDevMethod Method { get; }

        public override ICollection<IDevGraphNodeParameter> Inputs { get; }

        public override ICollection<IDevGraphNodeParameter> Outputs { get; }
    }
}
