using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Method
{
    public interface IDevMethodCollection: ICollection<IDevMethod>
    {
        
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

        IDevMethod GetMethod(string name, IDevType[] parameters);

    }
}
