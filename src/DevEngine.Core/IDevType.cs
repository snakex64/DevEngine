using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core
{
    public interface IDevType
    {
        string TypeName { get; }

        string TypeNamespace { get; }

        string TypeNamespaceAndName => $"{TypeNamespace}.{TypeName}";

        bool IsClass { get; }

        bool CanBeAssignedTo(IDevType type);
    }
}
