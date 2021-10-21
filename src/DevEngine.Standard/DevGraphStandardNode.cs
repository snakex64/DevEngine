using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard
{
    public abstract class DevGraphStandardNode : IDevGraphNode
    {
        public abstract bool IsExecNode { get; }

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public abstract bool ExecuteExecAsSubGraph { get; }

        public string Name { get; }

        public IDictionary<string, string> AdditionalContent { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, object?> AdditionalContentToBeSerialized { get; set; } = new Dictionary<string, object?>();

        public Guid Id { get; }

        public virtual int AmountOfDifferentVersions => 0;

        public virtual int Version
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        protected DevGraphStandardNode(Guid id, string name)
        {
            Id = id;
            Name = name;

            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();
        }

        public abstract DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance);

        public abstract IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance);

        public virtual void InitializeAfterPreLoad()
        {
            if (!AdditionalContent.TryGetValue("ConstantInputs", out var constantsStr))
                return;

            var constants = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(constantsStr);
            if (constants == null)
                return;
            // load the constant values
            foreach (var input in Inputs)
            {
                if (input.Type.IsBasicType)
                {
                    if (constants.TryGetValue(input.Name, out var constant))
                        input.ConstantValueStr = constant;
                }
            }
        }

        public virtual void OnParameterConnectionChanged(IDevGraphNodeParameter devGraphNodeParameter)
        {
        }

    }
}
