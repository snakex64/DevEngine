using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard.Base
{
    [ConstantSearchProvider]
    public class Constant<T> : DevGraphStandardNode
    {
        public Constant(Guid id, string name, IDevProject project) : base(id, name)
        {
            Inputs.Add(project.CreateGraphNodeParameter("C", project.GetRealType<T>(), true, this));

            Outputs.Add(project.CreateGraphNodeParameter("V", project.GetRealType<T>(), false, this));
        }

        public override bool IsExecNode => false;

        public override bool ExecuteExecAsSubGraph => false;

        public override DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            devGraphNodeInstance.Parameters[Outputs.First()] = devGraphNodeInstance.Parameters[Inputs.First()];

            return DevGraphNodeExecuteResult.Continue;
        }

        public override IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new NotImplementedException();
        }
    }


    public class ConstantSearchProviderAttribute : DevGraphNodeSearchProviderAttribute
    {
        public override IDevGraphNodeSearchProvider GetProvider(Type declaringType)
        {
            return new ConstantSearchProvider();
        }
    }

    public class ConstantSearchProvider : IDevGraphNodeSearchProvider
    {
        public IEnumerable<DevGraphNodeSearchResult> Search(string content)
        {
            var types = new[]
            {
                typeof(int),
                typeof(short),
                typeof(float),
                typeof(double),
                typeof(byte),
                typeof(long),
                typeof(string),
            };

            return types.Where( x=> string.IsNullOrEmpty(content) || ("Constant_" + x.Name).Contains(content, StringComparison.OrdinalIgnoreCase)).Select(x => new DevGraphNodeSearchResult("Constant_" + x.Name, "Constant_" + x.Name, (id, name, project) =>
            {
                var type = typeof(Constant<>);

                var genericType = type.MakeGenericType(x) ?? throw new Exception("Unable to make generic type fo Constant");

                return (IDevGraphNode?)Activator.CreateInstance(genericType, id, name.Replace("Constant_", ""), project) ?? throw new Exception("Unable to create Constant node");

            }, null)).ToList();
        }
    }
}
