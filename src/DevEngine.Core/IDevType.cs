using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core
{
    public interface IDevType
    {
        IDevProject Project { get; }

        string TypeName { get; }

        string TypeNamespace { get; }

        string TypeNamespaceAndName => $"{TypeNamespace}.{TypeName}";

        bool IsClass { get; }

        bool IsStruct { get; }

        bool IsEnum { get; }

        bool IsRealType { get; }

        bool IsBasicType { get; }

        bool IsUnknownedType { get; }

        bool CanBeAssignedTo(IDevType type);
    }
}
