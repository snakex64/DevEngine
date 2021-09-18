using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;

namespace DevEngine.Standard.Math
{
    [StandardSearchProvider("Add", "Add to base types")]
    public class AddNode<T> : BinaryMathNode<T>
    {
        public AddNode(Guid id, string name, IDevProject project) : base(id, name, project)
        {
        }

        public override T DoMath(dynamic a, dynamic b)
        {
            return a + b;
        }
    }
}
