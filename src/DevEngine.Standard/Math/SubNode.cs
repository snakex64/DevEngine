using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;

namespace DevEngine.Standard.Math
{
    [StandardSearchProvider("Sub", "Subtract two base types")]
    public class SubNode<T> : BinaryMathNode<T>
    {
        public SubNode(Guid id, string name, IDevProject project) : base(id, name, project)
        {
        }

        public override T DoMath(dynamic a, dynamic b)
        {
            return a - b;
        }

    }
}
