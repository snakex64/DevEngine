using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core
{
    public interface IDevType
    {
        string TypeName { get; }

        string TypeNamespace { get; }

        bool IsClass { get; }
    }
}
