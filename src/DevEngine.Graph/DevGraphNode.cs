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
        protected DevGraphNode()
        {
            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();
        }

        public string Name { get; set; }

        public bool IsExecNode => Inputs.Any(x => x.Type is Core.DevExecType);

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public DevGraphNodeType DevGraphNodeType => throw new NotImplementedException();
    }
}
