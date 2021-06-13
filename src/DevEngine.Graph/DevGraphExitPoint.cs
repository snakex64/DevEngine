using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphExitPoint : IDevGraphExitPoint
    {
        public DevGraphExitPoint()
        {
            Name = "Exit";

            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();

            Inputs.Add(new DevGraphNodeParameter(true, Core.DevExecType.ExecType, "Exec", this));
        }

        public string Name { get; }

        public bool IsExecNode => true;

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public IDictionary<string, string> AdditionalContent { get; } = new Dictionary<string, string>();

        public IDictionary<string, Func<string?>> AdditionalContentProvider { get; } = new Dictionary<string, Func<string?>>();

        public bool ExecuteExecAsSubGraph => false;

        public DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            return DevGraphNodeExecuteResult.Exit;
        }

        public IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new NotImplementedException();
        }
    }
}
