using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevSavedGraphNode
    {
        public Guid Id { get; set; }

        public string TypeFullName { get; set; } = null!;

        public List<string> GenericTypes { get; set; } = new List<string>();

        public string Name { get; set; } = null!;

        public IDictionary<string, string> AdditionalContent { get; set; } = null!;

        public List<DevSavedGraphNodeParameterConnection> ParameterConnections { get; set; } = null!;
    }

    public class DevSavedGraphNodeParameterConnection
    {
        public string LocalNodeParameterName { get; set; } = null!;

        public Guid DistantNodeId { get; set; }

        public string DistantNodeParameterName { get; set; } = null!;
    }
}
