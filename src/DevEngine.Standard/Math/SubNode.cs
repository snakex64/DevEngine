using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;

namespace DevEngine.Standard.Math
{
    public class SubNode<T> : BinaryMathNode<T>
    {
        public SubNode(string name, IDevProject project) : base(name, project)
        {
        }

        public override T DoMath(dynamic a, dynamic b)
        {
            return a - b;
        }

    }
}
