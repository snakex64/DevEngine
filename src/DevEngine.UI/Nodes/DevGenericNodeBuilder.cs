using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using DevEngine.RealTypes.Class;
using DevEngine.Standard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevEngine.UI.Nodes
{
    public class DevGenericNodeBuilder : Standard.DevGraphStandardNode
    {
        private Type? GenericNodeType;

        private readonly IDevProject Project;

        private List<GenericParameterResult>? Parameters;

        /// <param name="genericNodeType">is required when building this class manually</param>
        public DevGenericNodeBuilder(Guid id, string name, IDevProject project, Type? genericNodeType = null) : base(id, name)
        {
            Project = project;

            if (genericNodeType != null)
            {
                GenericNodeType = genericNodeType;

                Parameters = Initialize();

                AdditionalContent["GenericNodeType"] = genericNodeType.AssemblyQualifiedName;
            }
        }

        public override void InitializeAfterPreLoad()
        {
            base.InitializeAfterPreLoad();

            GenericNodeType = Type.GetType(AdditionalContent["GenericNodeType"]);

            if (GenericNodeType == null)
                throw new Exception("Unable to load GenericNodeType:" + AdditionalContent["GenericNodeType"]);

            Parameters = Initialize();
        }

        public override bool IsExecNode => false;

        public override bool ExecuteExecAsSubGraph => false;


        public override DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new System.NotImplementedException("Cannot execute generic node builder");
        }

        public override IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new System.NotImplementedException("Cannot execute generic node builder");
        }


        #region Initialize

        private List<GenericParameterResult> Initialize()
        {
            var parameters = SearchGenericParameterProvider();

            foreach(var parameter in parameters)
            {
                var collection = parameter.IsInput ? Inputs : Outputs;
                collection.Add(Project.CreateGraphNodeParameter(parameter.Name, parameter.KnownedType ?? Project.GetRealType<UnknownedType>(), parameter.IsInput, this));
            }

            return parameters;
        }

        #endregion

        #region SearchGenericParameterProvider

        private List<GenericParameterResult> SearchGenericParameterProvider()
        {
            if (GenericNodeType == null)
                throw new Exception("DevGenericNodeBuilder was not initialized correctly, missing GenericNodeType");

            var methods = GenericNodeType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);

            var provider = methods
                .ToDictionary(x => x, x => (GenericParameterProviderAttribute?)x.GetCustomAttributes(typeof(GenericParameterProviderAttribute), true)
                .FirstOrDefault())
                .Where(x => x.Value != null)
                .FirstOrDefault();

            if (provider.Value == null)
                throw new Exception("Unable to find GenericParameterProvider in type:" + GenericNodeType.Name);

            var fakeTemporaryTypeToGetGenericParameters = provider.Value.GetGenericParametersType;

            var method = fakeTemporaryTypeToGetGenericParameters.GetMethod(provider.Key.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if( method == null)
                throw new Exception("Unable to find GenericParameterProvider method in type:" + GenericNodeType.Name);

            var parameters = (List<GenericParameterResult>?)method.Invoke(null, null);

            if( parameters == null)
                throw new Exception("Unable to find GenericParameterProvider, returning null in type:" + GenericNodeType.Name);

            return parameters;

        }

        #endregion
    }

}
