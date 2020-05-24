using DevEngine.Core.Class;
using System;

namespace DevEngine.Core.Project
{
    public interface IDevProject
    {
        IDevClassCollection Classes { get; }
    }
}
