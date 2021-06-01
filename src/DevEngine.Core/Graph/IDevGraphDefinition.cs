using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphDefinition
    {
        string Name { get; }

        IDevGraphEntryPoint EntryPoint { get; }

        ICollection<IDevGraphExitPoint> ExitPoints { get; }

        ICollection<IDevGraphNode> Nodes { get; }
    }
}
