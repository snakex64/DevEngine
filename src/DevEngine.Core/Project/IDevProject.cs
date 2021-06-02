using DevEngine.Core.Class;
using System;

namespace DevEngine.Core.Project
{
    public interface IDevProject
    {
        IDevClassCollection Classes { get; }

        IDevType GetRealType(Type type);

        IDevType GetRealType<T>();

        IDevType GetVoidType();
    }
}
