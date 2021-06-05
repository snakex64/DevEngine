using DevEngine.Core.Class;
using DevEngine.Core.Graph;
using System;

namespace DevEngine.Core.Project
{
    public interface IDevProject
    {
        IDevClassCollection Classes { get; }

        IDevType GetRealType(Type type);

        IDevType GetRealType<T>();

        IDevType GetVoidType();

        IDevGraphNodeParameter CreateGraphNodeParameter(string name, IDevType devType, bool isInput, IDevGraphNode owner);
    }
}
