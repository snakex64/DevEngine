using DevEngine.Core;
using DevEngine.Core.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.FakeTypes.Property
{
    public class DevProperty : IDevProperty
    {
        public DevProperty(IDevType propertyType, string name, Visibility getVisibility = Visibility.Private, Visibility setVisibility = Visibility.Private)
        {
            if (setVisibility < GetVisibility)
                throw new Exception("SetVisibility cannot be more restrictive than GetVisibility");

            PropertyType = propertyType;
            Name = name;
            GetVisibility = getVisibility;
            SetVisibility = setVisibility;
        }

        public IDevType PropertyType { get; }

        public string Name { get; }

        public Visibility GetVisibility { get; }

        public Visibility SetVisibility { get; }
    }
}
