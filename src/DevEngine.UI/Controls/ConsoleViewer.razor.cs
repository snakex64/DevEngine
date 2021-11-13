using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevEngine.UI.Controls
{
    public partial class ConsoleViewer: ComponentBase, IDisposable
    {
        private List<string> Lines = new List<string>();


        [Inject]
        public Services.ConsoleService ConsoleService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ConsoleService.OnConsoleLinesChanged += ConsoleService_OnConsoleLinesChanged;
        }

        private void ConsoleService_OnConsoleLinesChanged()
        {
            InvokeAsync(() =>
            {
                Lines = ConsoleService.GetLines().ToList();

                StateHasChanged();
            });
        }

        public void Dispose()
        {
            ConsoleService.OnConsoleLinesChanged -= ConsoleService_OnConsoleLinesChanged;
        }

        private void OnClearClicked()
        {
            Lines.Clear();
            ConsoleService.Clear();
        }

    }
}
