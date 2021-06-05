using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using DevEngine.Evaluator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard
{
    public abstract class DevGraphStandardNode : IDevGraphStandardNode
    {
        public string Name { get; }

        public abstract bool IsExecNode { get; }

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public abstract bool ExecuteExecAsSubGraph { get; }

        public DevGraphNodeType DevGraphNodeType => DevGraphNodeType.StandardNode;

        protected DevGraphStandardNode(string name)
        {
            Name = name;

            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();
        }

        public abstract void Execute(IDevGraphNodeInstance devGraphNodeInstance);

        public abstract IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance);
    }
}
