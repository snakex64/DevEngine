using DevEngine.Core.Class;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Method
{
    public interface IDevMethod
    {
        IDevType DeclaringType { get; }

        public string Name { get; }

        public bool IsStatic { get; }

        IList<IDevMethodParameter> Parameters { get; }

        Visibility Visibility { get; }
    }
}
