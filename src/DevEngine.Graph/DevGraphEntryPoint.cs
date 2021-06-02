using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphEntryPoint : DevGraphNode, IDevGraphEntryPoint
    {
        public DevGraphEntryPoint()
        {
            Outputs.Add(new DevGraphNodeParameter(false, Core.DevExecType.ExecType, "Exec", this));
        }

        bool IDevGraphNode.IsExecNode => true;
    }
}
