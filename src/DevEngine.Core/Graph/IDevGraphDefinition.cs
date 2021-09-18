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

        void RemoveNode(IDevGraphNode node)
        {
            Nodes.Remove(node);
        }

        /// <summary>
        /// Does nothing special but validating really basic checkups, if both are inputs, if they are in the same node, etc..
        /// Direction doesn't matter, input to output or output to input will both work
        /// </summary>
        void ConnectNodesParameters(IDevGraphNodeParameter nodeParameter1, IDevGraphNodeParameter nodeParameter2);

        void DisconnectNodesParameters(IDevGraphNodeParameter nodeParameter1, IDevGraphNodeParameter nodeParameter2);

        /// <summary>
        /// used to save content with the graph definition
        /// </summary>
        IDictionary<string, string> AdditionalContent { get; }

        /// <summary>
        /// set objects here, they'll be saved with this graph definition
        /// If a Value is null, it won't be saved and any previously saved value associated with this Key will be removed
        /// </summary>
        IDictionary<string, object?> AdditionalContentToBeSerialized { get; }

        string Save();
    }
}
