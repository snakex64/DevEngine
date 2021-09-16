using DevEngine.Core.Graph;
using DevEngine.UI.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class SearchContextualPopup : ComponentBase
    {
        private readonly Services.NodeSearchService NodeSearchService;

        private string SearchText { get; set; } = "";

        [Parameter]
        [EditorRequired]
        public double MenuX { get; set; }

        [Parameter]
        [EditorRequired]
        public double MenuY { get; set; }

        private List<DevGraphNodeSearchResult> Results = new List<DevGraphNodeSearchResult>();

        [Parameter]
        public EventCallback<DevGraphNodeSearchResult> OnResultSelected { get; set; }

        [Parameter]
        public EventCallback OnCloseMenuSelected { get; set; }

        public SearchContextualPopup(NodeSearchService nodeSearchService)
        {
            NodeSearchService = nodeSearchService;
        }


        private void OnSearchTextChanged(string search)
        {
            SearchText = search;

            RefreshSearchResults();
        }

        private void RefreshSearchResults()
        {
            Results = NodeSearchService.Search(SearchText).ToList();

            StateHasChanged();
        }

        private Task OnSearchResultClicked(DevGraphNodeSearchResult result)
        {
            return OnResultSelected.InvokeAsync(result);
        }

        private Task CloseMenu()
        {
            return OnCloseMenuSelected.InvokeAsync();
        }

    }
}
