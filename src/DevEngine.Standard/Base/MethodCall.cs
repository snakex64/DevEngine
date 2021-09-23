﻿using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Graph;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using DevEngine.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevEngine.Standard.Math
{
    [MethodCallSearchProvider]
    public class MethodCall : DevGraphStandardNode
    {
        public MethodCall(Guid id, string name, IDevProject project, string? assemblyQualifiedName = null, string? methodName = null) : base(id, name)
        {
            Project = project;

            // if not provided, we're being pre-loaded so we just don't do it
            if (assemblyQualifiedName != null && methodName != null)
            {
                var devType = (IDevClass)project.GetRealType(Type.GetType(assemblyQualifiedName) ?? throw new Exception("Unable to find type:" + assemblyQualifiedName));

                DevMethods = devType.Methods.Where(x => x.Name == methodName).ToList();

                CurrentSavedMethodCallInfo = new SavedMethodCallInfo()
                {
                    AssemblyQualifiedName = assemblyQualifiedName,
                    MethodName = methodName,
                };

                ChooseMethod(DevMethods.First());

            }
        }

        public override bool IsExecNode => true;

        public override bool ExecuteExecAsSubGraph => false; // methods always only have 1 output exec nodes

        public IList<IDevMethod>? DevMethods { get; set; }

        public IDevMethod? MethodChoosen { get; set; }

        private IDevProject Project { get; }

        private SavedMethodCallInfo? CurrentSavedMethodCallInfo { get; set; }

        public override int AmountOfDifferentVersions => DevMethods?.Count ?? 0;

        public override int Version
        {
            get
            {
                if (DevMethods == null)
                    throw new Exception("DevMethods shouldn't be null here");

                if (MethodChoosen == null)
                    throw new Exception("MethodChoosen shouldn't be null here");

                return DevMethods.IndexOf(MethodChoosen) + 1;
            }
            set
            {
                if (DevMethods == null)
                    throw new Exception("DevMethods shouldn't be null here");

                ChooseMethod(DevMethods[value - 1]);
            }
        }

        public void ChooseMethod(IDevMethod devMethod)
        {
            if (DevMethods == null)
                throw new Exception("DevMethods shouldn't be null here");

            if (!DevMethods.Contains(devMethod))
                throw new Exception("DevMethods doesn't contain the method we're currently trying to choose");


            AdditionalContentToBeSerialized[nameof(MethodCall)] = CurrentSavedMethodCallInfo ?? throw new Exception("CurrentSavedMethodCallInfo shouldn't be null here");

            // remove previous inputs and ouputs
            Inputs.Clear();
            Outputs.Clear();


            Inputs.Add(Project.CreateGraphNodeParameter("_In", Project.ExecType, true, this));
            Outputs.Add(Project.CreateGraphNodeParameter("_Out", Project.ExecType, false, this));

            // if the method is not a static call, add the "self" / "this" input parameter
            if (!devMethod.IsStatic)
                Inputs.Add(Project.CreateGraphNodeParameter("self", devMethod.DeclaringType, true, this));

            foreach (var parameter in devMethod.Parameters)
            {
                var collection = parameter.IsOut ? Outputs : Inputs;

                collection.Add(Project.CreateGraphNodeParameter(parameter.Name, parameter.ParameterType, !parameter.IsOut, this));
            }

            CurrentSavedMethodCallInfo.Parameters = Inputs.Where(x => x.Name != "self").Concat(Outputs).ToDictionary(x => x.Name, x => new SavedTypeName(x.Type));

            MethodChoosen = devMethod;
        }

        public override void InitializeAfterPreLoad()
        {
            base.InitializeAfterPreLoad();

            CurrentSavedMethodCallInfo = JsonSerializer.Deserialize<SavedMethodCallInfo>(AdditionalContent[nameof(MethodCall)]) ?? throw new Exception("Unable to parse SavedMethodCallInfo");

            AdditionalContentToBeSerialized[nameof(MethodCall)] = CurrentSavedMethodCallInfo;

            var devType = (IDevClass)Project.GetRealType(Type.GetType(CurrentSavedMethodCallInfo.AssemblyQualifiedName) ?? throw new Exception("Unable to find type:" + CurrentSavedMethodCallInfo.AssemblyQualifiedName));

            DevMethods = devType.Methods.Where(x => x.Name == CurrentSavedMethodCallInfo.MethodName).ToList();

            IDevMethod? savedMethod = null;

            foreach (var method in DevMethods)
            {
                if (method.Parameters.Count != CurrentSavedMethodCallInfo.Parameters.Count)
                    continue;

                var same = method.Parameters.All(parameter => CurrentSavedMethodCallInfo.Parameters[parameter.Name].TryGetDevType(Project, out var savedType) ? false : parameter.ParameterType == savedType);
                if (same)
                {
                    savedMethod = method;
                    break;
                }
            }

            if (savedMethod == null)
                savedMethod = DevMethods.First();

            ChooseMethod(savedMethod);

        }

        public override DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            devGraphNodeInstance.Parameters[Outputs.First()] = devGraphNodeInstance.Parameters[Inputs.First()];

            return DevGraphNodeExecuteResult.Continue;
        }

        public override IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new NotImplementedException();
        }

        private class SavedMethodCallInfo
        {
            public string AssemblyQualifiedName { get; set; } = null!;

            public string MethodName { get; set; } = null!;

            public Dictionary<string, SavedTypeName> Parameters { get; set; } = null!;
        }
    }


    public class MethodCallSearchProviderAttribute : DevGraphNodeSearchProviderAttribute
    {
        public override IDevGraphNodeSearchProvider GetProvider(Type declaringType)
        {
            return new MethodCallSearchProvider();
        }
    }

    public class MethodCallSearchProvider : IDevGraphNodeSearchProvider
    {
        public IEnumerable<DevGraphNodeSearchResult> Search(string content)
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
            {
                // for now, let's keep it easy
                if (!type.IsClass || type.IsGenericType)
                    continue;

                foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static).GroupBy(x => x.Name))
                {
                    var fullName = type.Namespace + "." + type.Name + "." + method.Key;
                    if (!string.IsNullOrEmpty(content) && !(fullName).Contains(content, StringComparison.OrdinalIgnoreCase))
                        continue;

                    yield return new DevGraphNodeSearchResult(fullName, fullName, (id, name, project) =>
                    {

                        return new MethodCall(id, name, project, type.AssemblyQualifiedName, method.Key);
                    }, null);
                }


            }
        }
    }
}
