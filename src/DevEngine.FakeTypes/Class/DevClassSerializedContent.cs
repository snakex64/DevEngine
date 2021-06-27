using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.FakeTypes.Class
{
    public class DevClassSerializedContent
    {
        public string FullNameWithNamespace { get; set; } = null!;

        public string? ParentClassFullNameWithNamespace { get; set; }

        public Core.Visibility Visibility { get; set; }
    }
}
