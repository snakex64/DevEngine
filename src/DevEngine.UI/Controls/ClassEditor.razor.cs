using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class ClassEditor : ComponentBase
    {
        [Parameter]
        public FakeTypes.Class.DevClass DevClass { get; set; } = null!;

    }
}
