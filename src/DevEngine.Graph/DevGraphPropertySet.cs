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
        public DevGraphPropertySet(IDevProperty property)
        {
            Property = property;
        }

        public IDevProperty Property { get; }
    }
}
