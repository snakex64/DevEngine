using DevEngine.Core.Class;
using DevEngine.Core.Evaluator;
using DevEngine.Core.Graph;
using System;
using System.Threading.Tasks;

namespace DevEngine.Core.Project
{
    public interface IDevProject
    {
        string Name { get; }

        IDevClassCollection Classes { get; }

        IDevType GetRealType(Type type);

        IDevType GetRealType<T>();

        IDevType ExecType {  get; }

        IDevType GetVoidType();

        IDevGraphNodeParameter CreateGraphNodeParameter(string name, IDevType devType, bool isInput, IDevGraphNode owner);

        void Save(string folder, bool updateProjectFolder);

        void Load(string folder);

        void RenameClass(string oldFullNameWithNamespace, string newFullNameWithNamespace);

        Task RunAsConsole(IDevGraphEvaluator evaluator, IConsoleLogger? consoleLogger);
    }
}
