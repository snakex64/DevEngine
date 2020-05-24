using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public interface IDevClassCollection : IDictionary<DevClassName, IDevClass>
    {
        void Add(IDevClass devClass)
        {
            Add(devClass.Name, devClass);
        }

        IDevClass this[string fullName] => this[new DevClassName(fullName)];



        List<DevClassName> GetClassesInNamespace(string @namespace, bool includeSubNamespaces = false);
    }
}
