using DevEngine.Core;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.RealTypes
{
    public class RealTypesProviderService : IRealTypesProviderService
    {
        private Dictionary<Type, IDevType> CachedTypes { get; } = new Dictionary<Type, IDevType>();

        public IDevType GetDevType(IDevProject project, Type type)
        {
            if (CachedTypes.TryGetValue(type, out var cachedType))
                return cachedType;

            if (type.IsClass || type.IsValueType)
            {
                var realClass = new Class.RealClass(project, type, this)
                {
                    IsClass = type.IsClass,
                    IsStruct = type.IsValueType
                };

                return CachedTypes[type] = realClass;
            }
            else if (type.IsEnum)
                return CachedTypes[type] = new DevEnum(project, type);
            else
                throw new NotImplementedException("Cannot create wrapper around real type:" + type.FullName);
        }
    }
}
