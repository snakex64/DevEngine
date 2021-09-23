﻿using DevEngine.Core;
using DevEngine.Core.Graph;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Standard.Base
{
    public class SelfNode : DevGraphStandardNode
    {
        public SelfNode(string name, IDevGraphDefinition devDefinition, IDevProject project) : this(Guid.Empty, name, devDefinition, project)
        {
        }

        public SelfNode(Guid id, string name, IDevGraphDefinition devDefinition, IDevProject project) : base(id, name)
        {

            SelfType = devDefinition.OwningType;

            Outputs.Add(SelfParameterNode = project.CreateGraphNodeParameter("Self", SelfType, false, this));
        }

        public override bool IsExecNode => false;

        public override bool ExecuteExecAsSubGraph => false;

        public IDevType SelfType { get; }

        private IDevGraphNodeParameter SelfParameterNode;

        public override DevGraphNodeExecuteResult Execute(IDevGraphNodeInstance devGraphNodeInstance)
        {
            devGraphNodeInstance.Parameters[SelfParameterNode] = devGraphNodeInstance.DevGraphInstance.Self;

            return DevGraphNodeExecuteResult.Continue;
        }

        public override IDevGraphNodeParameter GetNextExecutionParameter(IDevGraphNodeInstance devGraphNodeInstance)
        {
            throw new NotImplementedException();
        }
    }
}
