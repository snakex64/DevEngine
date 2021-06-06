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
            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();

            Outputs.Add(ExecNode = new DevGraphNodeParameter(false, Core.DevExecType.ExecType, "Exec", this));
        }

        public string Name { get; }

        public bool IsExecNode => true;

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

        public bool ExecuteExecAsSubGraph => false;

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
