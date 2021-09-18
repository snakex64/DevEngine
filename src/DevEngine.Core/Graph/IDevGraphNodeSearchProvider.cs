using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Core.Graph
{
    public interface IDevGraphNodeSearchProvider
    {
        IEnumerable<DevGraphNodeSearchResult> Search(string content);

    }

    public abstract class DevGraphNodeSearchProviderAttribute : Attribute
    {
        public abstract IDevGraphNodeSearchProvider GetProvider(Type declaringType);
    }

    public class DevGraphNodeSearchResult
    {
        public DevGraphNodeSearchResult(string displayName, string? description, Func<Guid, string, IDevProject, IDevGraphNode> buildNewNode, Type? genericType)
        {
            DisplayName = displayName;
            Description = description;
            BuildNewNode = buildNewNode;
            GenericType = genericType;
        }

        public string DisplayName { get; }

        public string? Description { get; }

        public Func<Guid, string, IDevProject, IDevGraphNode> BuildNewNode { get; }

        public Type? GenericType { get; }
    }
}
