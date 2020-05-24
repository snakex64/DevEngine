using DevEngine.Core;
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

        static internal DevMethodCollection CreateFromType(Type type, RealTypesProviderService realTypesProviderService)
        {
            var methods = new HashSet<IDevMethod>();

            foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                methods.Add(new DevMethod(realTypesProviderService, method));
            }

            return new DevMethodCollection(methods);
        }

        private DevMethodCollection(HashSet<IDevMethod> methods)
        {
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
