using DevEngine.Core.Graph;
using DevEngine.Core.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphPropertySet : DevGraphNode, IDevGraphPropertySet
    {
        public DevGraphPropertySet(IDevProperty property, IEnumerable<IDevGraphNodeParameter> inputs, IEnumerable<IDevGraphNodeParameter> outputs)
        {
            Property = property;
            Inputs = inputs.ToList();
            Outputs = outputs.ToList();
        }

        public IDevProperty Property { get; }

        public override ICollection<IDevGraphNodeParameter> Inputs { get; }

        public override ICollection<IDevGraphNodeParameter> Outputs { get; }
    }
}
