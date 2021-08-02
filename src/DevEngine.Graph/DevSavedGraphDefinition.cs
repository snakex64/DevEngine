using DevEngine.Core;
using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Graph
{
    public class DevSavedGraphDefinition
    {
        public string Name { get; set; } = null!;

        public IDictionary<string, string> AdditionalContent { get; set; } = null!;

        public DevGraphDefinitionType DefinitionType { get; set; }

        public SavedTypeName OwningType { get; set; } = null!;

        public string OwningMemberName { get; set; } = null!;

        public string? DefinitionTypeName { get; set; }

        public IDictionary<Guid, DevSavedGraphNode> Nodes { get; set; } = null!;
    }
}
