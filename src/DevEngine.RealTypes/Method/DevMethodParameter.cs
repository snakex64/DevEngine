using DevEngine.Core;
using DevEngine.Core.Method;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.RealTypes.Method
{
    public class DevMethodParameter : IDevMethodParameter
    {
        public DevMethodParameter(IDevType parameterType, string name, bool isOut, bool isRef)
        {
            if (IsOut && IsRef)
                throw new Exception("Cannot be both out and ref parameter: " + name);

            ParameterType = parameterType;
            Name = name;
            IsOut = isOut;
            IsRef = isRef;
        }

        public IDevType ParameterType { get; }

        public string Name { get; }

        public bool IsOut { get; }

        public bool IsRef { get; }
    }
}
