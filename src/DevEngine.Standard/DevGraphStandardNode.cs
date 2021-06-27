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
        public string Name { get; }

        public abstract bool IsExecNode { get; }

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public abstract bool ExecuteExecAsSubGraph { get; }

        public IDictionary<string, string> AdditionalContent { get; } = new Dictionary<string, string>();

        public IDictionary<string, object?> AdditionalContentToBeSerialized { get; } = new Dictionary<string, object?>();

        protected DevGraphStandardNode(string name)
        {
            Name = name;

            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();
        }

        public abstract DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance);

        public abstract IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance);
    }
}
