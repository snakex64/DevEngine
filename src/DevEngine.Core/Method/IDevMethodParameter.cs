using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Method
{
    public interface IDevMethodParameter
    {
        IDevType ParameterType { get; }

        string Name { get; }

        bool IsOut { get; }

        bool IsRef { get; }
    }
}
