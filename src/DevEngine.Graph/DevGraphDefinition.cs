using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphDefinition : IDevGraphDefinition
    {
        public DevGraphDefinition(string name, IEnumerable<IDevGraphNode> nodes)
        {
            Name = name;
            Nodes = nodes.ToList();
        }

        public string Name { get; }

        public ICollection<IDevGraphNode> Nodes { get; }
    }
}
