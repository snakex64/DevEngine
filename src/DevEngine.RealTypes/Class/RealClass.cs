using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Property;
using DevEngine.RealTypes.Method;
using DevEngine.RealTypes.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.RealTypes.Class
{
    public class RealClass : IDevClass
    {
        #region Declarations

        internal RealClass(Type realType, RealTypesProviderService realTypesProviderService)
        {
            RealType = realType;
            RealTypesProviderService = realTypesProviderService;

            Name = new DevClassName(RealType.Namespace, RealType.Name);

            LazyProperties = new Lazy<IDevPropertyCollection>(() => DevPropertyCollection.CreateFromType(RealType, RealTypesProviderService));
            LazyMethods = new Lazy<IDevMethodCollection>(() => DevMethodCollection.CreateFromType(RealType, RealTypesProviderService));
        }

        private Type RealType { get; }

        private RealTypesProviderService RealTypesProviderService { get; }

        public IDevType? BaseType => RealType.BaseType != null ? RealTypesProviderService.GetDevType(RealType.BaseType) : null;

        public DevClassName Name { get; }

        private Lazy<IDevMethodCollection> LazyMethods { get; }
        public IDevMethodCollection Methods => LazyMethods.Value;

        private Lazy<IDevPropertyCollection> LazyProperties { get; }
        public IDevPropertyCollection Properties => LazyProperties.Value;

        public Visibility Visibility => Visibility.Public;

        public string TypeName => Name.Name;

        public string TypeNamespace => Name.Namespace;

        public bool IsClass => true;

        #endregion

        public bool CanBeAssignedTo(IDevType type)
        {
            if (type == this)
                return true;

            if (type is RealClass realClass)
                return realClass.RealType.IsAssignableFrom(RealType);

            if (BaseType != null)
                return BaseType.CanBeAssignedTo(type);

            return false;
        }
    }
}
