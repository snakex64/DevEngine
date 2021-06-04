using DevEngine.Core;
using DevEngine.Core.Project;
using System;

namespace DevEngine.RealTypes
{
    public interface IRealTypesProviderService
    {
        IDevType GetDevType(IDevProject project, Type type);
    }
}