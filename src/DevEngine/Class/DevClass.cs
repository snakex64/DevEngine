using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Property;
using DevEngine.Method;
using DevEngine.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Class
{
    public class DevClass : IDevClass
    {
        #region Declarations

        public DevClass(IDevType? baseType, DevClassName name)
        {
            BaseType = baseType;
            Name = name;
            Methods = new DevMethodCollection(this);
        }

        public IDevType? BaseType { get; }

        public DevClassName Name { get; }

        public IDevMethodCollection Methods { get; }

        public IDevPropertyCollection Properties { get; } = new DevPropertyCollection();

        public Visibility Visibility { get; }

        public string TypeName => Name.Name;

        public string TypeNamespace => Name.Namespace;

        public bool IsClass => true;

        #endregion

        public bool CanBeAssignedTo(IDevType type)
        {
            if (!type.IsClass)
                return false;

            if (type == this)
                return true;

            if (BaseType != null)
                return BaseType.CanBeAssignedTo(type);

            return false;
        }
    }
}
