using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    /// <summary>
    /// Represent getting a property
    /// IsExecNode should always be false
    /// There should only be one input node, the object from wich to get the property
    /// There should only be one output node, the property value we're getting
    /// </summary>
    public interface IDevGraphPropertyGet: IDevGraphNode
    {
        Property.IDevProperty Property { get; }
    }
}
