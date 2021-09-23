using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.RealTypes.Method
{
    internal class DevMethodCollection: IDevMethodCollection
    {
        public IDevClass DevClass { get; }

        static internal DevMethodCollection CreateFromType(IDevClass devClass, Type type, RealTypesProviderService realTypesProviderService)
        {
            var methods = new HashSet<IDevMethod>();

            foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static))
                methods.Add(new DevMethod(devClass.Project, realTypesProviderService, method));

            return new DevMethodCollection(devClass, methods);
        }

        private DevMethodCollection(IDevClass devClass, HashSet<IDevMethod> methods)
        {
            DevClass = devClass;
            Methods = methods;

        }

        #region Collection

        public int Count => Methods.Count;

        public bool IsReadOnly => true;

        private IReadOnlyCollection<IDevMethod> Methods { get; }


        public void Add(IDevMethod item)
        {
            throw new Exception("Cannot modify real types");
        }

        public void Clear()
        {
            throw new Exception("Cannot modify real types");
        }

        public bool Contains(IDevMethod item)
        {
            return Methods.Contains(item);
        }

        public void CopyTo(IDevMethod[] array, int arrayIndex)
        {
            ((ICollection<IDevMethod>)Methods).CopyTo(array, arrayIndex);
        }

        public IEnumerator<IDevMethod> GetEnumerator()
        {
            return Methods.GetEnumerator();
        }

        public bool Remove(IDevMethod item)
        {
            throw new Exception("Cannot modify real types");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Methods.GetEnumerator();
        }

        #endregion
    }
}
