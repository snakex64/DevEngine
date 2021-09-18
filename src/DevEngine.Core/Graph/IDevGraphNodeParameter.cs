using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNodeParameter
    {
        IDevGraphNode ParentNode { get; }

        bool IsInput { get; }

        bool IsOutput => !IsInput;

        IDevType Type { get; set; }

        string Name { get; }

        ICollection<IDevGraphNodeParameter> Connections { get; }

        /// <summary>
        /// Only defined if <seealso cref="IsInput"/> is true.
        /// This is set when the user write a constant value in the UI.
        /// It is not set to null even if the parameter is connected afterward, to keep the value for convenience
        /// </summary>
        public string? ConstantValueStr { get; set; }
    }
}
