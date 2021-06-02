using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphDefinition : IDevGraphDefinition
    {
        public DevGraphDefinition(IDevProject project, string name)
        {
            Name = name;
            Nodes = new HashSet<IDevGraphNode>();
            Project = project;
        }

        public string Name { get; }

        public IDevProject Project { get; }

        public ICollection<IDevGraphNode> Nodes { get; }

        public IDevGraphEntryPoint EntryPoint => (IDevGraphEntryPoint)Nodes.Single(x => x is IDevGraphEntryPoint);

        public IEnumerable<IDevGraphExitPoint> ExitPoints => Nodes.OfType<IDevGraphExitPoint>();


        #region InitializeEmptyForMethod

        public void InitializeEmptyForMethod(IDevMethod devMethod)
        {
            var entry = new DevGraphEntryPoint();

            foreach (var parameter in devMethod.Parameters)
            {
                if (!parameter.IsOut)
                    entry.Outputs.Add(new DevGraphNodeParameter(false, parameter.ParameterType, parameter.Name, entry));
            }

            Nodes.Add(entry);

            var exit = new DevGraphExitPoint();

            foreach (var parameter in devMethod.Parameters)
            {
                if (parameter.IsOut)
                    exit.Inputs.Add(new DevGraphNodeParameter(true, parameter.ParameterType, parameter.Name, exit));
            }

            if (devMethod.ReturnType != Project.GetVoidType())
                exit.Inputs.Add(new DevGraphNodeParameter(true, devMethod.ReturnType, "return", exit));

            Nodes.Add(exit);
        }

        #endregion

        #region ConnectNodesParameters

        public void ConnectNodesParameters(IDevGraphNodeParameter nodeParameter1, IDevGraphNodeParameter nodeParameter2)
        {
            if (nodeParameter1.IsInput == nodeParameter2.IsInput)
                throw new Exception("Cannot connect input to input or output to output");

            if (nodeParameter1.ParentNode == nodeParameter2.ParentNode)
                throw new Exception("Cannot connect to yourself");

            if (nodeParameter1.Connections.Contains(nodeParameter2))
                return; // we're already connected, nothing to do

            nodeParameter1.Connections.Add(nodeParameter2);
            nodeParameter2.Connections.Add(nodeParameter1);
        }

        #endregion

        #region DisconnectNodes

        public void DisconnectNodesParameters(IDevGraphNodeParameter nodeParameter1, IDevGraphNodeParameter nodeParameter2)
        {
            nodeParameter1.Connections.Remove(nodeParameter2);
            nodeParameter2.Connections.Remove(nodeParameter1);
        }

        #endregion
    }
}
