using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using DevEngine.Core.Property;
using DevEngine.FakeTypes.Method;
using DevEngine.FakeTypes.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.FakeTypes.Class
{
    public class DevClass : IDevClass
    {

        public DevClass(IDevProject project, IDevType? baseType, DevClassName name, string folder)
        {
            Project = project;
            BaseType = baseType;
            Name = name;
            Methods = new DevMethodCollection(this);
            Folder = folder;
        }

        public IDevType? BaseType { get; }

        public DevClassName Name { get; }

        public IDevMethodCollection Methods { get; }

        public IDevPropertyCollection Properties { get; } = new DevPropertyCollection();

        public Visibility Visibility { get; set; }

        public string TypeName => Name.Name;

        public string TypeNamespace => Name.Namespace;

        public bool IsClass => true;

        public IDevProject Project { get; }

        public bool ShouldBeSaved => true;

        public string Folder { get; set; }

        private DevClassSerializedContent? PreloadedSerializedContent;

        public bool CanBeAssignedTo(IDevType type)
        {
            if (!type.IsClass)
                return false;

            if (type == this)
                return true;

            return BaseType?.CanBeAssignedTo(type) ?? false;
        }

        internal static DevClass Preload(Project.DevProject devProject, string file, Project.DevProjectSerializedContent projectSerializedContent)
        {
            var serializedContent = System.Text.Json.JsonSerializer.Deserialize<DevClassSerializedContent>(file);

            if (serializedContent == null)
                throw new Exception("Unable to deserialize DevClass during Preload:" + file);

            var parentClassName = serializedContent.ParentClassFullNameWithNamespace != null ? (DevClassName?)new DevClassName(serializedContent.ParentClassFullNameWithNamespace) : null;
            var parentClass = parentClassName == null ? null : devProject.PreloadClass(parentClassName.Value, projectSerializedContent);

            return new DevClass(devProject, parentClass, new DevClassName(serializedContent.FullNameWithNamespace), System.IO.Path.GetDirectoryName(file) ?? throw new Exception("Unable to get directory name from file:" + file))
            {
                Visibility = serializedContent.Visibility
            };
        }

        public void Save(string file)
        {
            var serializedContent = new DevClassSerializedContent()
            {
                FullNameWithNamespace = Name.FullNameWithNamespace,
                ParentClassFullNameWithNamespace = BaseType?.TypeNamespaceAndName,
                Visibility = Visibility
            };

            System.IO.File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(serializedContent));
        }
    }
}
