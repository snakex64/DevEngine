using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Core.Evaluator
{

    public interface IDevGraphEvaluator
    {
        void Evaluate(DevObject self, Method.IDevMethod devMethod, Dictionary<string, DevObject> inputs, out Dictionary<string, DevObject> outputs);
    }
}
