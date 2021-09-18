using DevEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard
{
    public class GenericParameterProviderAttribute : Attribute
    {
        public GenericParameterProviderAttribute(Type getGenericParametersType)
        {
            GetGenericParametersType = getGenericParametersType;
        }

        public Type GetGenericParametersType { get; set; }
    }

    public class GenericParameterResult
    {
        public GenericParameterResult(string name, string genericName, bool isInput)
        {
            Name = name;
            GenericName = genericName;
            IsInput = isInput;
        }

        public GenericParameterResult(string name, IDevType knownedType, bool isInput)
        {
            Name = name;
            KnownedType = knownedType;
            IsInput = isInput;
        }

        public string Name { get; }

        public string? GenericName { get; }

        public IDevType? KnownedType { get; }

        public bool IsInput { get; }
    }
}
