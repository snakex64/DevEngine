using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphEntryPoint :  IDevGraphEntryPoint
    {
        public DevGraphEntryPoint()
        {
            Name = "Entry";

            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();

            Outputs.Add(ExecNode = new DevGraphNodeParameter(false, Core.DevExecType.ExecType, "Exec", this));
        }

        public string Name { get; }

        public bool IsExecNode => true;

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public bool ExecuteExecAsSubGraph => false;

        public IDictionary<string, string> AdditionalContent { get; } = new Dictionary<string, string>();

        public IDictionary<string, object?> AdditionalContentToBeSerialized { get; } = new Dictionary<string, object?>();

        private IDevGraphNodeParameter ExecNode;

        public DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            return DevGraphNodeExecuteResult.Continue;
        }

        public IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            return ExecNode;
        }
    }
}
