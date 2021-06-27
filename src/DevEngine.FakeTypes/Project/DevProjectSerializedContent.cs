using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.FakeTypes.Project
{
    public class DevProjectSerializedContent
    {
        public DevProjectSerializedContent(string name, Dictionary<string, string> classes)
        {
            Name = name;
            Classes = classes;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DevProjectSerializedContent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string Name { get; set; }

        /// <summary>
        /// contains a list of all the classes in the project, Key is the full name with namespace, Value is the relative path to the file itself
        /// </summary>
        public Dictionary<string, string> Classes { get; set; } = null!;
    }
}
