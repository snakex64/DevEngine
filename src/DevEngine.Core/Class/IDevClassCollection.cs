using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public interface IDevClassCollection : IDictionary<DevClassName, IDevClass>
    {

        List<DevClassName> GetClassesInNamespace(string @namespace, bool includeSubNamespaces = false);
    }
}
