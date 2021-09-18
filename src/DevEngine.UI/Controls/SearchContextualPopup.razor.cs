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
        [Inject]
        private NodeSearchService NodeSearchService { get; set; } = null!;

        private string SearchText { get; set; } = "";

        private List<DevGraphNodeSearchResult> Results = new List<DevGraphNodeSearchResult>();

        [Parameter]
        public EventCallback<DevGraphNodeSearchResult> OnResultSelected { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
                RefreshSearchResults();
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

    }
}
