using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using DevEngine.Evaluator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard.Math
{
    public abstract class BinaryMathNode<T> : DevGraphStandardNode
    {
        public override bool IsExecNode => false;

        public override bool ExecuteExecAsSubGraph => false;

        private IDevGraphNodeParameter Parameter_A;
        private IDevGraphNodeParameter Parameter_B;

        private IDevGraphNodeParameter OutputNode;

        protected BinaryMathNode(string name, IDevProject project) : base(name)
        {
            Inputs.Add(Parameter_A = project.CreateGraphNodeParameter("A", project.GetRealType<T>(), true, this));
            Inputs.Add(Parameter_B = project.CreateGraphNodeParameter("B", project.GetRealType<T>(), true, this));

            Outputs.Add(OutputNode = project.CreateGraphNodeParameter("V", project.GetRealType<T>(), false, this));
        }

        public override void Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            if (Parameter_A == null || Parameter_B == null || OutputNode == null)
                throw new Exception("Inputs aren't initialized");

            var parameterA = devGraphNodeInstance.Parameters[Parameter_A];
            var parameterB = devGraphNodeInstance.Parameters[Parameter_B];

            dynamic? a = parameterA.Value;
            dynamic? b = parameterB.Value;


            if (a == null || b == null)
                devGraphNodeInstance.Parameters[OutputNode] = new DevObject(parameterA.DevType, null);
            else
            {
                T v = DoMath(a, b);
                devGraphNodeInstance.Parameters[OutputNode] = new DevObject(parameterA.DevType, v);
            }
        }

        public abstract T DoMath(dynamic a, dynamic b);

        public override IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new NotImplementedException();
        }
    }
}
