using DevEngine.Core.Property;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.FakeTypes.Property
{
    internal class DevPropertyCollection: IDevPropertyCollection
    {
        #region Dictionary

        IDictionary<string, IDevProperty> Properties = new Dictionary<string, IDevProperty>();

        public IDevProperty this[string key] { get => Properties[key]; set => Properties[key] = value; }

        public ICollection<string> Keys => Properties.Keys;

        public ICollection<IDevProperty> Values => Properties.Values;

        public int Count => Properties.Count;

        public bool IsReadOnly => Properties.IsReadOnly;

        public void Add(string key, IDevProperty value)
        {
            Properties.Add(key, value);
        }

        public void Add(KeyValuePair<string, IDevProperty> item)
        {
            Properties.Add(item);
        }

        public void Clear()
        {
            Properties.Clear();
        }

        public bool Contains(KeyValuePair<string, IDevProperty> item)
        {
            return Properties.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, IDevProperty>[] array, int arrayIndex)
        {
            Properties.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, IDevProperty>> GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return Properties.Remove(key);
        }

        public bool Remove(KeyValuePair<string, IDevProperty> item)
        {
            return Properties.Remove(item);
        }

        public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out IDevProperty value)
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
