using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;

namespace DevEngine.Standard.Math
{
    public class AddNode<T> : BinaryMathNode<T>
    {
        public AddNode(string name, IDevProject project) : base(name, project)
        {
        }

        public override T DoMath(dynamic a, dynamic b)
        {
            return a + b;
        }
    }
}
