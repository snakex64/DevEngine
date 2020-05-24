using DevEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.RealTypes
{
    public class RealTypesProviderService
    {

        private Dictionary<Type, IDevType> CachedTypes { get; } = new Dictionary<Type, IDevType>();

        public IDevType GetDevType(Type type)
        {
            if (CachedTypes.TryGetValue(type, out var cachedType))
                return cachedType;

            if (type.IsClass || type.IsValueType)
                return CachedTypes[type] = new RealTypes.Class.RealClass(type, this);
            else if (type.IsEnum)
                return CachedTypes[type] = new DevEnum(type);
            throw new NotImplementedException("Cannot create wrapper around real type:" + type.FullName);
        }
    }
}
