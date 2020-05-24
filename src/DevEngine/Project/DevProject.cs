using DevEngine.Class;
using DevEngine.Core.Class;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Project
{
    public class DevProject : IDevProject
    {
        public IDevClassCollection Classes { get; } = new DevClassCollection();
    }
}
