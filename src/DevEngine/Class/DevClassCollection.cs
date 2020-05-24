using DevEngine.Core.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Class
{
    internal class DevClassCollection : IDevClassCollection
    {
        public List<DevClassName> GetClassesInNamespace(string @namespace, bool includeSubNamespaces = false)
        {
            var names = new List<DevClassName>();

            foreach (var name in Keys)
            {
                if (name.IsInNamespace(@namespace, includeSubNamespaces))
                    names.Add(name);
            }

            return names;
        }

        #region Dictionary

        public IDevClass this[DevClassName key]
        {
            get => Classes[key];
            set => Classes[key] = value;
        }

        public ICollection<DevClassName> Keys => Classes.Keys;

        public ICollection<IDevClass> Values => Classes.Values;

        public int Count => Classes.Count;

        public bool IsReadOnly => false;

        private Dictionary<DevClassName, IDevClass> Classes { get; } = new Dictionary<DevClassName, IDevClass>();

        public void Add(DevClassName key, IDevClass value)
        {
            Classes.Add(key, value);
        }

        public void Add(KeyValuePair<DevClassName, IDevClass> item)
        {
            ((IDictionary<DevClassName, IDevClass>)Classes).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<DevClassName, IDevClass>)Classes).Clear();
        }

        public bool Contains(KeyValuePair<DevClassName, IDevClass> item)
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).Contains(item);
        }

        public bool ContainsKey(DevClassName key)
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<DevClassName, IDevClass>[] array, int arrayIndex)
        {
            ((IDictionary<DevClassName, IDevClass>)Classes).CopyTo(array, arrayIndex);
        }


        public IEnumerator<KeyValuePair<DevClassName, IDevClass>> GetEnumerator()
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).GetEnumerator();
        }

        public bool Remove(DevClassName key)
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).Remove(key);
        }

        public bool Remove(KeyValuePair<DevClassName, IDevClass> item)
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).Remove(item);
        }

        public bool TryGetValue(DevClassName key, out IDevClass value)
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<DevClassName, IDevClass>)Classes).GetEnumerator();
        }

        #endregion
    }
}
