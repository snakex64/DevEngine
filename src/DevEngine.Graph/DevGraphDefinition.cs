using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphDefinition : IDevGraphDefinition
    {
        public DevGraphDefinition(IDevProject project, string name, IDevType owningType, DevGraphDefinitionType definitionType, string owningMemberName)
        {
            Name = name;
            Nodes = new HashSet<IDevGraphNode>();
            Project = project;
            OwningType = owningType;
            DefinitionType = definitionType;
            OwningMemberName = owningMemberName;
        }

        public string Name { get; }

        public IDevProject Project { get; }

        public ICollection<IDevGraphNode> Nodes { get; }

        public IDevGraphEntryPoint EntryPoint => (IDevGraphEntryPoint)Nodes.Single(x => x is IDevGraphEntryPoint);

        public IEnumerable<IDevGraphExitPoint> ExitPoints => Nodes.OfType<IDevGraphExitPoint>();

        public IDevType OwningType { get; }

        public DevGraphDefinitionType DefinitionType { get; }

        public string OwningMemberName { get; }

        public IDictionary<string, string> AdditionalContent { get; } = new Dictionary<string, string>();

        public IDictionary<string, object?> AdditionalContentToBeSerialized { get; } = new Dictionary<string, object?>();


        #region InitializeEmptyForMethod

        public void InitializeEmptyForMethod(IDevMethod devMethod)
        {
            var entry = new DevGraphEntryPoint(Guid.NewGuid());

            foreach (var parameter in devMethod.Parameters)
            {
                if (!parameter.IsOut)
                    entry.Outputs.Add(new DevGraphNodeParameter(false, parameter.ParameterType, parameter.Name, entry));
            }

            Nodes.Add(entry);

            var exit = new DevGraphExitPoint(Guid.NewGuid());

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

        #region Save

        public virtual string Save()
        {
            foreach (var additionalContent in AdditionalContentToBeSerialized)
            {
                if (additionalContent.Value != null)
                    AdditionalContent[additionalContent.Key] = JsonSerializer.Serialize(additionalContent.Value);
                else
                    AdditionalContent.Remove(additionalContent.Key);
            }

            var nodes = new Dictionary<Guid, DevSavedGraphNode>();
            foreach (var node in Nodes)
                nodes[node.Id] = SaveNode(node);

            var savedDefinition = new DevSavedGraphDefinition()
            {
                Name = Name,
                AdditionalContent = AdditionalContent,
                DefinitionType = DefinitionType,
                OwningType = new SavedTypeName(OwningType),
                OwningMemberName = OwningMemberName,
                DefinitionTypeName = GetType().FullName ?? throw new Exception("FullName couldn't be generate"),
                Nodes = nodes
            };
            return JsonSerializer.Serialize(savedDefinition);
        }

        #region SaveNode

        private DevSavedGraphNode SaveNode(IDevGraphNode node)
        {
            var constantInputs = new Dictionary<string, string>();

            foreach( var input in node.Inputs)
            {
                if( input.ConstantValueStr != null )
                    constantInputs[input.Name] = input.ConstantValueStr;
            }
            node.AdditionalContentToBeSerialized["ConstantInputs"] = constantInputs;

            foreach (var additionalContent in node.AdditionalContentToBeSerialized)
            {
                if (additionalContent.Value != null)
                    node.AdditionalContent[additionalContent.Key] = JsonSerializer.Serialize(additionalContent.Value);
                else
                    node.AdditionalContent.Remove(additionalContent.Key);
            }


            return new DevSavedGraphNode()
            {
                TypeFullName = node.GetType().AssemblyQualifiedName ?? throw new Exception("Unable to get type full name"),
                Id = node.Id,
                Name = node.Name,
                AdditionalContent = node.AdditionalContent,
                ParameterConnections = node.Outputs.SelectMany(y => y.Connections.Select(z => new DevSavedGraphNodeParameterConnection()
                {
                    DistantNodeId = z.ParentNode.Id,
                    DistantNodeParameterName = z.Name,
                    LocalNodeParameterName = y.Name
                })).ToList()
            };
        }

        #endregion

        #endregion

        #region Load

        public static DevGraphDefinition Load(string serializedGraph, IDevProject project, IDevMethod owningMethod)
        {
            var savedGraphDefinition = JsonSerializer.Deserialize<DevSavedGraphDefinition>(serializedGraph);
            if (savedGraphDefinition == null)
                throw new Exception("Unable to deserialize savedGraphDefinition");

            if (!savedGraphDefinition.OwningType.TryGetDevType(project, out var owningType))
                throw new Exception("Unable to get type:" + (savedGraphDefinition.OwningType.FullNetClassName ?? savedGraphDefinition.OwningType.FullDevClassName));

            var graphDefinition = new DevGraphDefinition(project, savedGraphDefinition.Name, owningType, savedGraphDefinition.DefinitionType, savedGraphDefinition.OwningMemberName);

            // first create and add all the nodes
            foreach (var node in savedGraphDefinition.Nodes)
            {
                var type = Type.GetType(node.Value.TypeFullName);

                if (type == null)
                    throw new Exception("Unable to get node type:" + node.Value.TypeFullName);

                var constructor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
                if (constructor == null)
                    throw new Exception("Unable to find constructor for node type:" + node.Value.TypeFullName);

                var constructorParametersDefinition = constructor.GetParameters();

                var parameters = new object[constructorParametersDefinition.Length];

                for (int i = 0; i < constructorParametersDefinition.Length; i++)
                {
                    var parameterDefinition = constructorParametersDefinition[i];


                    parameters[i] = parameterDefinition.Name switch
                    {
                        "name" => node.Value.Name,
                        "id" => node.Key,
                        "project" => project,
                        _ => parameterDefinition.HasDefaultValue ? null! : throw new Exception("Unable to find parameter to fit in node constructor:" + parameterDefinition.Name),
                    };
                }

                var nodeInstance = (IDevGraphNode?)Activator.CreateInstance(type, parameters);
                if (nodeInstance == null)
                    throw new Exception("Unable to create node instance:" + node.Value.TypeFullName);

                foreach (var additionalContent in node.Value.AdditionalContent)
                    nodeInstance.AdditionalContent.Add(additionalContent);

                if (nodeInstance is DevGraphEntryPoint entry)
                {
                    foreach (var parameter in owningMethod.Parameters)
                    {
                        if (!parameter.IsOut)
                            entry.Outputs.Add(new DevGraphNodeParameter(false, parameter.ParameterType, parameter.Name, entry));
                    }

                }
                else if (nodeInstance is DevGraphExitPoint exit)
                {
                    foreach (var parameter in owningMethod.Parameters)
                    {
                        if (parameter.IsOut)
                            exit.Inputs.Add(new DevGraphNodeParameter(true, parameter.ParameterType, parameter.Name, exit));
                    }

                    if (owningMethod.ReturnType != project.GetVoidType())
                        exit.Inputs.Add(new DevGraphNodeParameter(true, owningMethod.ReturnType, "return", exit));
                }

                graphDefinition.Nodes.Add(nodeInstance);

                nodeInstance.InitializeAfterPreLoad();
            }

            // then connect the nodes together
            foreach (var node in savedGraphDefinition.Nodes)
            {
                var currentNode = graphDefinition.Nodes.First(x => x.Id == node.Value.Id);

                foreach (var connection in node.Value.ParameterConnections)
                {
                    var otherNode = graphDefinition.Nodes.First(x => x.Id == connection.DistantNodeId);

                    var currentNodeParameter = currentNode.Outputs.First(x => x.Name == connection.LocalNodeParameterName);
                    var distantNodeParameter = otherNode.Inputs.First(x => x.Name == connection.DistantNodeParameterName);

                    graphDefinition.ConnectNodesParameters(currentNodeParameter, distantNodeParameter);
                }
            }

            return graphDefinition;
        }

        #endregion
    }
}
