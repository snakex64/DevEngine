using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Core.Graph
{
    public enum DevGraphDefinitionType
    {
        Method
    }
    public interface IDevGraphDefinition
    {
        IDevProject Project { get; }

        string Name { get; }

        IDevType OwningType { get; }

        DevGraphDefinitionType DefinitionType { get; }

        string OwningMemberName { get; }

        IDevGraphEntryPoint EntryPoint { get; }

        IEnumerable<IDevGraphExitPoint> ExitPoints { get; }

        ICollection<IDevGraphNode> Nodes { get; }

        void AddNode(IDevGraphNode node)
        {
            Nodes.Add(node);
        }

        /// <summary>
        /// Does nothing special but validating really basic checkups, if both are inputs, if they are in the same node, etc..
        /// Direction doesn't mapper, input to output or output to input will both work
        /// </summary>
        void ConnectNodesParameters(IDevGraphNodeParameter nodeParameter1, IDevGraphNodeParameter nodeParameter2);
    }
}
