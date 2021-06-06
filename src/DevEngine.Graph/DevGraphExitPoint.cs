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
            Inputs = new List<IDevGraphNodeParameter>();
            Outputs = new List<IDevGraphNodeParameter>();

            Inputs.Add(new DevGraphNodeParameter(true, Core.DevExecType.ExecType, "Exec", this));
        }

        public string Name { get; }

        public bool IsExecNode => false;

        public ICollection<IDevGraphNodeParameter> Inputs { get; }

        public ICollection<IDevGraphNodeParameter> Outputs { get; }

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
