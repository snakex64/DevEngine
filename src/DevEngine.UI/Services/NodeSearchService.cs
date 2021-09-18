using DevEngine.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.UI.Services
{
    public class NodeSearchService
    {
        private ICollection<IDevGraphNodeSearchProvider>? Providers;


        public IEnumerable<DevGraphNodeSearchResult> Search(string text)
        {
            if (Providers == null)
            {
                InitializeProviders();

                if (Providers == null)
                    throw new Exception("Unable to initialize search providers");
            }

            foreach (var provider in Providers)
            {
                var results = provider.Search(text);

                foreach (var result in results)
                    yield return result;
            }
        }

        private void InitializeProviders()
        {
            // patch, since the standard engine is never used statically, it won't be loaded, so let's just hit it once to load it!
            var hit = typeof(Standard.SelfNode);

            var providers = new List<IDevGraphNodeSearchProvider>();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
            {
                var attribute = type.GetCustomAttributes(typeof(DevGraphNodeSearchProviderAttribute), true).FirstOrDefault();
                if (attribute is DevGraphNodeSearchProviderAttribute providerAttribute)
                {
                    var provider = providerAttribute.GetProvider(type);
                    if (provider != null)
                        providers.Add(provider);
                }
            }

            Providers = providers;
        }
    }
}
