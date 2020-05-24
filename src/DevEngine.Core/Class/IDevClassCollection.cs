using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public interface IDevClassCollection : IDictionary<DevClassName, IDevClass>
    {

        List<DevClassName> GetClassesInNamespace(string @namespace, bool includeSubNamespaces = false)
        {
            var names = new List<DevClassName>();

            foreach (var name in Keys)
            {
                if (name.IsInNamespace(@namespace, includeSubNamespaces))
                    names.Add(name);
            }

            return names;
        }
    }
}
