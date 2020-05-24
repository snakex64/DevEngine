using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Property
{
    public interface IDevProperty
    {
        IDevType PropertyType { get; }

        string Name { get; }

        Visibility GetVisibility { get; }

        Visibility SetVisibility { get; }
    }
}
