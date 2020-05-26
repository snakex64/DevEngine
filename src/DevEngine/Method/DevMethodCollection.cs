using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Method
{
    internal class DevMethodCollection: IDevMethodCollection
    {
        public DevMethodCollection(IDevClass devClass)
        {
            DevClass = devClass;
        }

        public IDevClass DevClass { get; }

        #region Collection

        public int Count => Methods.Count;

        public bool IsReadOnly => Methods.IsReadOnly;

        private ICollection<IDevMethod> Methods { get; } = new HashSet<IDevMethod>();

        public void Add(IDevMethod item)
        {
            Methods.Add(item);
        }

        public void Clear()
        {
            Methods.Clear();
        }

        public bool Contains(IDevMethod item)
        {
            return Methods.Contains(item);
        }

        public void CopyTo(IDevMethod[] array, int arrayIndex)
        {
            Methods.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IDevMethod> GetEnumerator()
        {
            return Methods.GetEnumerator();
        }

        public bool Remove(IDevMethod item)
        {
            return Methods.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Methods.GetEnumerator();
        }

        #endregion
    }
}
