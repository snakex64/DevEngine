using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard
{
    public class StandardSearchProvider : IDevGraphNodeSearchProvider
    {
        private readonly Type TypeToInstanciate;

        private readonly string Name;

        private readonly string? Description;

        internal StandardSearchProvider(Type typeToInstanciate, string name, string? description)
        {
            TypeToInstanciate = typeToInstanciate;
            Name = name;
            Description = description;
        }

        public IEnumerable<DevGraphNodeSearchResult> Search(string content)
        {
            if (Name.Contains(content, StringComparison.OrdinalIgnoreCase) || Description?.Contains(content, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new[]
                {
                    new DevGraphNodeSearchResult(Name, Description, CreateNodeInstance, TypeToInstanciate.ContainsGenericParameters ? TypeToInstanciate : null)
                };
            }

            return Array.Empty<DevGraphNodeSearchResult>();
        }

        private IDevGraphNode CreateNodeInstance(Guid id, string name, Core.Project.IDevProject project)
        {
            var constructor = TypeToInstanciate.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            if (constructor == null)
                throw new Exception("Unable to find constructor for node type:" + TypeToInstanciate.FullName);

            var constructorParametersDefinition = constructor.GetParameters();

            var parameters = new object[constructorParametersDefinition.Length];

            for (int i = 0; i < constructorParametersDefinition.Length; i++)
            {
                var parameterDefinition = constructorParametersDefinition[i];

                parameters[i] = parameterDefinition.Name switch
                {
                    "name" => name,
                    "id" => id,
                    "project" => project,
                    _ => throw new Exception("Unable to find parameter to fit in node constructor:" + parameterDefinition.Name),
                };
            }

            var nodeInstance = (IDevGraphNode?)Activator.CreateInstance(TypeToInstanciate, parameters);

            if (nodeInstance == null)
                throw new Exception("Unable to instanciate node:" + TypeToInstanciate.FullName);

            return nodeInstance;
        }
    }

    public class StandardSearchProviderAttribute : DevGraphNodeSearchProviderAttribute
    {
        public StandardSearchProviderAttribute(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public override IDevGraphNodeSearchProvider GetProvider(Type declaringType)
        {
            return new StandardSearchProvider(declaringType, Name, Description);
        }
    }
}
