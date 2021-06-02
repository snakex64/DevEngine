using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphMethodCall : DevGraphNode, IDevGraphMethodCall
    {
        public DevGraphMethodCall(IDevMethod method)
        {
            Method = method;
        }

        public IDevMethod Method { get; }
    }
}
