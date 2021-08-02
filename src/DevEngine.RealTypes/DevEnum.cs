using DevEngine.Core;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.RealTypes
{
    internal class DevEnum : IDevType
    {
        internal DevEnum(IDevProject project, Type type)
        {
            Project = project;

            if (!type.IsEnum)
                throw new Exception("Type is not an enum:" + type.FullName);

            TypeName = type.Name;

            TypeNamespace = type.Namespace ?? "";
        }

        public string TypeName { get; }

        public string TypeNamespace { get; }

        public bool IsClass => false;

        public IDevProject Project { get; }

        public bool IsStruct => false;

        public bool IsEnum => true;

        public bool CanBeAssignedTo(IDevType type)
        {
            return type == this;
        }
    }
}
