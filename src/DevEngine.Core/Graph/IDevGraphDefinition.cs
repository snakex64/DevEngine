using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphDefinition
    {
        string Name { get; }

        public IDevGraphEntryPoint EntryPoint => (IDevGraphEntryPoint)Nodes.Single(x => x is IDevGraphEntryPoint);

        IEnumerable<IDevGraphExitPoint> ExitPoints => Nodes.OfType<IDevGraphExitPoint>();

        ICollection<IDevGraphNode> Nodes { get; }
    }
}
