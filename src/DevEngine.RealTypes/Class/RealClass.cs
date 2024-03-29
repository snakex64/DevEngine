using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
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

        internal RealClass(IDevProject project, Type realType, RealTypesProviderService realTypesProviderService)
        {
            Project = project;
            RealType = realType;
            RealTypesProviderService = realTypesProviderService;

            Name = new DevClassName(RealType.Namespace ?? "", RealType.Name);

            LazyProperties = new Lazy<IDevPropertyCollection>(() => DevPropertyCollection.CreateFromType(project, RealType, RealTypesProviderService));
            LazyMethods = new Lazy<IDevMethodCollection>(() => DevMethodCollection.CreateFromType(this, RealType, RealTypesProviderService));
        }

        public IDevProject Project { get; }

        public Type RealType { get; }

        private RealTypesProviderService RealTypesProviderService { get; }

        public IDevType? BaseType => RealType.BaseType != null ? RealTypesProviderService.GetDevType(Project, RealType.BaseType) : null;

        public DevClassName Name { get; }

        private Lazy<IDevMethodCollection> LazyMethods { get; }
        public IDevMethodCollection Methods => LazyMethods.Value;

        private Lazy<IDevPropertyCollection> LazyProperties { get; }
        public IDevPropertyCollection Properties => LazyProperties.Value;

        public Visibility Visibility => Visibility.Public;

        public string TypeName => Name.Name;

        public string TypeNamespace => Name.Namespace;

        public bool IsClass { get; internal set; }

        public bool ShouldBeSaved => false;

        public string Folder => throw new NotImplementedException();

        public bool IsStruct { get; internal set; }

        public bool IsEnum => false;

        public bool IsRealType => true;

        public bool IsUnknownedType => RealType == typeof(UnknownedType);

        public bool IsBasicType => RealType.IsPrimitive || RealType == typeof(string);

        #endregion


        public bool CanBeAssignedTo(IDevType type)
        {
            if (IsUnknownedType) // we can be assigned to anything, except other unknowned type
            {
                if (type is RealClass realClassType) // if real class, we have to make sure it's not an unknowned type
                    return !realClassType.IsUnknownedType;

                // if it's fake class, then sure, we can assign to
                return type is not DevExecType;
            }
            else if (type.IsUnknownedType)
                return true; // any non-generic can be plugged into a generic

            if (type == this)
                return true;

            if (type is RealClass realClass)
                return realClass.RealType.IsAssignableFrom(RealType);

            if (BaseType != null)
                return BaseType.CanBeAssignedTo(type);

            return false;
        }

        public void Save(string file)
        {
            throw new NotImplementedException();
        }

        public void Load(string file)
        {
            throw new NotImplementedException();
        }
    }
}
