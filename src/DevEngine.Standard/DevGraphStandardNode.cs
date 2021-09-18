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
        }

        public virtual void OnParameterConnectionChanged(IDevGraphNodeParameter devGraphNodeParameter)
        { 
        }
    }
}
