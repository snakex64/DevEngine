using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevGraphInputParameterGet : DevGraphNode, IDevGraphInputParameterGet
    {
        public IDevGraphNodeParameter ParameterToGet { get; }

        public DevGraphInputParameterGet(IDevGraphNodeParameter parameterToGet)
        {
            ParameterToGet = parameterToGet;
        }
    }
}
