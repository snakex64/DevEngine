using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent setting a property on an object
    /// IsExecNode should always be true
    /// There should only be two input node, the exec node and the value to set on the property
    /// There should only be one output node, the property new value
    /// </summary>
    public interface IDevGraphPropertySet: IDevGraphNode
    {
        Property.IDevProperty Property { get; }
    }
}
