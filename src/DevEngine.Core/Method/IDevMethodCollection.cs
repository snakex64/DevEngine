using DevEngine.Core.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.Core.Method
{
    public interface IDevMethodCollection: ICollection<IDevMethod>
    {
        IDevClass DevClass { get; }

        IDevMethod GetMethod(string name, IDevType[] parameters)
        {
            IEnumerable<IDevMethod> allMethods = this;
            if (DevClass.BaseType is IDevClass baseType && baseType != null)
                allMethods = allMethods.Concat(baseType.Methods);

            foreach (var method in allMethods)
            {
                if (method.Name != name)
                    continue;

                if (method.Parameters.Count != parameters.Length)
                    continue;

                bool found = true;
                for (var i = 0; i < parameters.Length; ++i)
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

        IDevMethod GetMethod(string name)
        {
            IDevMethod? method = null;

            foreach( var value in this)
            {
                if( value.Name == name)
                {
                    if (method != null)
                        throw new Exception("Ambiguity between multiple methods: " + name);
                    method = value;
                    // keep going to ensure we don't have multiple matches
                }
            }

            if (method == null)
                throw new Exception("Method not found:" + name);

            return method;
        }

    }
}
