using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Method
{
    public interface IDevMethodCollection: ICollection<IDevMethod>
    {

        IDevMethod GetMethod(string name, IDevType[] parameters)
        {
            foreach (var method in this)
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
                }
            }

            if (method == null)
                throw new Exception("Method not found:" + name);

            return method;
        }

    }
}
