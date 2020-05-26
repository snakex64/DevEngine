using DevEngine.Core.Property;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DevEngine.RealTypes.Property
{
    internal class DevPropertyCollection: IDevPropertyCollection
    {
        static internal DevPropertyCollection CreateFromType(Type type, RealTypesProviderService realTypesProviderService)
        {
            var collection = new Dictionary<string, IDevProperty>();

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy))
            {
                collection[property.Name] = new DevProperty(realTypesProviderService, property);
            }

            return new DevPropertyCollection(collection);
        }

        internal DevPropertyCollection(Dictionary<string, IDevProperty> properties)
        {
            Properties = properties;
        }

        #region Dictionary

        IReadOnlyDictionary<string, IDevProperty> Properties { get; }

        public IDevProperty this[string key] 
        { 
            get => Properties[key];
            set => throw new Exception("Cannot modify real types");
        }

        public ICollection<string> Keys => new ReadOnlyCollection<string>(Properties.Keys.ToList());

        public ICollection<IDevProperty> Values => new ReadOnlyCollection<IDevProperty>(Properties.Values.ToList());

        public int Count => Properties.Count;

        public bool IsReadOnly => true;

        public void Add(string key, IDevProperty value)
        {
            throw new Exception("Cannot modify real types");
        }

        public void Add(KeyValuePair<string, IDevProperty> item)
        {
            throw new Exception("Cannot modify real types");
        }

        public void Clear()
        {
            throw new Exception("Cannot modify real types");
        }

        public bool Contains(KeyValuePair<string, IDevProperty> item)
        {
            return Properties.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, IDevProperty>[] array, int arrayIndex)
        {
            ((IDictionary<string, IDevProperty>)Properties).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, IDevProperty>> GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        public bool Remove(string key)
        {
            throw new Exception("Cannot modify real types");
        }

        public bool Remove(KeyValuePair<string, IDevProperty> item)
        {
            throw new Exception("Cannot modify real types");
        }

        public bool TryGetValue(string key, out IDevProperty value)
        {
            return Properties.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        #endregion
    }
}
