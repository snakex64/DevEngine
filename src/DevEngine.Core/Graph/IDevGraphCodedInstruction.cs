using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent a node that can be executed directly or inlined in the generated code.
    /// Usually a pre-defined node type, like "a + b"
    /// </summary>
    public interface IDevGraphCodedInstruction : IDevGraphNode
    {
    }
}
