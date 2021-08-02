using DevEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.FakeTypes.Method
{
    internal class DevSavedMethod
    {
        public string Name { get; set; } = null!;

        public SavedTypeName ReturnType { get; set; } = null!;

        public List<DevSavedParameter> Parameters { get; set; } = null!;

        public string? SerializedGraph { get; set; }

        public bool IsStatic { get; set; }

        public Visibility Visibility { get; set; }
    }

    internal class DevSavedParameter
    {
        public string Name { get; set; } = null!;

        public SavedTypeName ParameterType { get; set; } = null!;

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

    }
}
