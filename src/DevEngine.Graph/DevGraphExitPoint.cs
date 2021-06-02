using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphExitPoint : DevGraphNode, IDevGraphExitPoint
    {
        public DevGraphExitPoint()
        {
            Inputs.Add(new DevGraphNodeParameter(true, Core.DevExecType.ExecType, "Exec", this));
        }
    }
}
