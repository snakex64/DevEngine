using DevEngine.Core.Method;
using DevEngine.Core.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public interface IDevClass: IDevType
    {
        IDevType? BaseType { get; }

        DevClassName Name { get; }

        IDevMethodCollection Methods { get; }

        IDevPropertyCollection Properties { get; }

        Visibility Visibility { get; }
    }
}
