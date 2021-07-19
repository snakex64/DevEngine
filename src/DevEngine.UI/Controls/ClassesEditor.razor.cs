using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class ClassesEditor: ComponentBase
    {
        private ICollection<FakeTypes.Class.DevClass> OpenedClasses = new List<FakeTypes.Class.DevClass>();

        private string? CurrentSelectedTab { get; set; }

        public void OpenClass(FakeTypes.Class.DevClass devClass)
        {
            if(!OpenedClasses.Contains(devClass))
                OpenedClasses.Add(devClass);

            CurrentSelectedTab = devClass.Name.FullNameWithNamespace;

            StateHasChanged();
        }
    }
}
