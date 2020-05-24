using DevEngine.Core;
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
        public IDevMethod GetMethod(string name, IDevType[] parameters)
        {
            foreach( var method in Methods)
            {
                if (method.Name != name)
                    continue;

                if (method.Parameters.Count != parameters.Length)
                    continue;

                bool found = true;
                for( var i = 0; i < parameters.Length; ++i)
                {
                    if (!parameters[i].CanBeAssignedTo(method.Parameters[i].ParameterType))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return method;
            }

            throw new Exception("Method not found with specified parameters: " + name);
        }


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
