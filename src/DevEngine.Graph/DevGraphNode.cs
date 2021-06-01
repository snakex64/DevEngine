using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public abstract class DevGraphNode : IDevGraphNode
    {
        public string Name { get; set; }

        public bool IsExecNode => Inputs.Any(x => x.Type is Core.DevExecType);

        public abstract ICollection<IDevGraphNodeParameter> Inputs { get; }

        public abstract ICollection<IDevGraphNodeParameter> Outputs { get; }
    }
}
