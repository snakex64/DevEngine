using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class TopMenu : ComponentBase
    {

        [Parameter]
        public EventCallback OnSaveRequested { get; set; }

        [Parameter]
        public EventCallback OnRunRequested { get; set; }

    }
}
