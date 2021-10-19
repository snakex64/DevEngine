using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevEngine.FakeTypes.Method
{
    public class DevMethod : IDevMethod
    {
        public DevMethod(IDevClass declaringClass, string name, bool isStatic, IDevType returnType, Visibility visibility = Visibility.Private)
        {
            DeclaringType = declaringClass;
            Name = name;
            IsStatic = isStatic;
            Visibility = visibility;
            ReturnType = returnType;
        }

        public IDevType DeclaringType { get; }

        public string Name { get; }

        public bool IsStatic { get; set; }

        public IList<IDevMethodParameter> Parameters { get; } = new List<IDevMethodParameter>();

        public Visibility Visibility { get; }

        public IDevType ReturnType { get; }

        public Core.Graph.IDevGraphDefinition? GraphDefinition { get; set; }

        public void AddInput(IDevMethodParameter parameter)
        {
            Parameters.Add(parameter);

            GraphDefinition?.AddInput(parameter.Name, parameter.ParameterType);
        }

        public void AddOutput(IDevMethodParameter parameter)
        {
            Parameters.Add(parameter);

            GraphDefinition?.AddOutput(parameter.Name, parameter.ParameterType);
        }

        internal DevSavedMethod Save()
        {
            var savedMethod = new DevSavedMethod
            {
                Name = Name,
                ReturnType = new SavedTypeName(ReturnType),
                SerializedGraph = GraphDefinition?.Save(),
                IsStatic = IsStatic,
                Visibility = Visibility,
                Parameters = Parameters.Select(x => new DevSavedParameter()
                {
                    Name = x.Name,
                    IsOut = x.IsOut,
                    IsRef = x.IsRef,
                    ParameterType = new SavedTypeName(x.ParameterType)
                }).ToList(),
            };

            return savedMethod;
        }
    }
}
