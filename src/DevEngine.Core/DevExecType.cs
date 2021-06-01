using DevEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core
{
    public class DevExecType : IDevType
    {
        public string TypeName => "Exec";

        public string TypeNamespace => "System";

        public bool IsClass => false;

        public bool CanBeAssignedTo(IDevType type)
        {
            return type is DevExecType;
        }
    }
}
