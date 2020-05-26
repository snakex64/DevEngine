using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevEngine.RealTypes.Method
{
    public class DevMethod : IDevMethod
    {
        public DevMethod(RealTypesProviderService realTypesProviderService, MethodInfo methodInfo)
        {
            DeclaringType = realTypesProviderService.GetDevType(methodInfo.DeclaringType);
            Name = methodInfo.Name;
            IsStatic = methodInfo.IsStatic;
            Visibility = Visibility.Private;
            ReturnType = realTypesProviderService.GetDevType(methodInfo.ReturnType);

            LazyParameters = new Lazy<IList<IDevMethodParameter>>(() =>
            {
                return new ReadOnlyCollection<IDevMethodParameter>(methodInfo.GetParameters().Select(x =>
                {
                    return new DevMethodParameter(realTypesProviderService.GetDevType(x.ParameterType), x.Name, x.IsOut, x.ParameterType.IsByRef);
                }).OfType<IDevMethodParameter>().ToList());
            });
        }

        public IDevType DeclaringType { get; }

        public string Name { get; }

        public bool IsStatic { get; }

        private Lazy<IList<IDevMethodParameter>> LazyParameters { get; }
        public IList<IDevMethodParameter> Parameters => LazyParameters.Value;

        public Visibility Visibility { get; }

        public IDevType ReturnType { get; }
    }
}
