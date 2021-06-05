using DevEngine.Core;
using DevEngine.Evaluator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator.Core
{
    public class DevObject
    {
        public DevObject(IDevType devType, object? value)
        {
            DevType = devType;
            Value = value;
        }

        public IDevType DevType { get; }

        public object? Value { get; }
    }
}
