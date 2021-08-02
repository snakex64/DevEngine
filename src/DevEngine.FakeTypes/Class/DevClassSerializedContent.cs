using DevEngine.Core;
using DevEngine.Core.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.FakeTypes.Class
{
    internal class DevClassSerializedContent
    {
        public string FullNameWithNamespace { get; set; } = null!;

        public SavedTypeName? ParentClass { get; set; }

        public Core.Visibility Visibility { get; set; }

        public string Folder { get; set; } = null!;

        public List<SavedProperty> Properties { get; set; }

        public List<Method.DevSavedMethod> Methods { get; set; }
    }


    public class SavedProperty
    {
        public string Name { get; set; } = null!;

        public SavedTypeName ClassName { get; set; } = null!;

        public Visibility GetVisibility { get; set; }

        public Visibility SetVisibility { get; set; }
    }
}
