using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI
{
    public static class Utility
    {
        public static IEnumerable<T> RecursiveChildren<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> childrenProvider)
        {
            foreach (var obj in enumerable)
            {
                yield return obj;

                foreach( var child in childrenProvider(obj).RecursiveChildren(childrenProvider))
                    yield return child;
            }
        }

    }
}
