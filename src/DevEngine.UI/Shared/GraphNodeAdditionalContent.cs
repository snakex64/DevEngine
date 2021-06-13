using DevEngine.Core.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Shared
{
    public class GraphNodeAdditionalContent
    {
        public GraphNodeAdditionalContent(IDevGraphNode devGraphNode)
        {
            if (devGraphNode.AdditionalContent.TryGetValue("DevEngine.UI.Shared.GraphNodeAdditionalContent", out var content))
                JsonConvert.PopulateObject(content, this);

            devGraphNode.AdditionalContentProvider["DevEngine.UI.Shared.GraphNodeAdditionalContent"] = SaveContent;
        }

        private string? SaveContent()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
