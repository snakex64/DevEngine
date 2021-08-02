using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using DevEngine.Core.Property;
using DevEngine.FakeTypes.Method;
using DevEngine.FakeTypes.Project;
using DevEngine.FakeTypes.Property;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool IsStruct => false;

        public bool IsEnum => false;

        public IDevProject Project { get; }

        public bool ShouldBeSaved => true;

        public string Folder { get; set; }

        public bool IsRealType => false;

        private DevClassSerializedContent? PreloadedSerializedContent;

        public bool CanBeAssignedTo(IDevType type)
        {
            if (type.IsClass != IsClass)
                return false;

            if (type.IsStruct != IsStruct)
                return false;

            if (type == this)
                return true;

            return BaseType?.CanBeAssignedTo(type) ?? false;
        }

        internal static DevClass Preload(Project.DevProject devProject, string file, Project.DevProjectSerializedContent projectSerializedContent)
        {
            var content = System.IO.File.ReadAllText(file);

            var serializedContent = System.Text.Json.JsonSerializer.Deserialize<DevClassSerializedContent>(content);

            if (serializedContent == null)
                throw new Exception("Unable to deserialize DevClass during Preload:" + file);


            var parentClass = serializedContent.ParentClass == null ? null : devProject.PreloadClass(serializedContent.ParentClass, projectSerializedContent);

            var devClass = new DevClass(devProject, parentClass, new DevClassName(serializedContent.FullNameWithNamespace), serializedContent.Folder)
            {
                Visibility = serializedContent.Visibility,
                PreloadedSerializedContent = serializedContent,
            };


            return devClass;
        }

        public void Save(string file)
        {
            var serializedContent = new DevClassSerializedContent()
            {
                FullNameWithNamespace = Name.FullNameWithNamespace,
                ParentClass = BaseType == null ? null : new SavedTypeName(BaseType),
                Visibility = Visibility,
                Properties = Properties.Select(x => new SavedProperty()
                {
                    ClassName = new SavedTypeName(x.Value.PropertyType),
                    Name = x.Key,
                    GetVisibility = x.Value.GetVisibility,
                    SetVisibility = x.Value.SetVisibility
                }).ToList(),
                Methods = Methods.OfType<DevMethod>().Select(x => x.Save()).ToList()
            };

            var path = System.IO.Path.GetDirectoryName(file) ?? throw new Exception("Unable to get directory from class file name");
            if(!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            System.IO.File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(serializedContent));
        }


        internal void LoadPropertiesAfterPreload(DevProject devProject)
        {
            if (PreloadedSerializedContent == null)
                throw new Exception("Class doesn't seems like it was preloaded?");

            if (PreloadedSerializedContent.Properties == null)
                return;

            foreach (var savedProperty in PreloadedSerializedContent.Properties)
            {
                var property = new DevProperty(devProject.GetTypeFromSavedClassName(savedProperty.ClassName), savedProperty.Name, savedProperty.GetVisibility, savedProperty.SetVisibility);

                Properties.Add(property.Name, property);
            }
        }

        internal void LoadMethodsAfterPreload(DevProject project)
        {
            if (PreloadedSerializedContent == null)
                throw new Exception("Class doesn't seems like it was preloaded?");

            if (PreloadedSerializedContent.Methods == null)
                return;

            foreach (var savedMethod in PreloadedSerializedContent.Methods)
            {
                if (savedMethod.ReturnType.TryGetDevType(project, out var returnType))
                {
                    var method = new DevMethod(this, savedMethod.Name, savedMethod.IsStatic, returnType, savedMethod.Visibility);

                    foreach (var parameter in savedMethod.Parameters)
                    {
                        if (parameter.ParameterType.TryGetDevType(project, out var parameterType))
                            method.Parameters.Add(new DevMethodParameter(parameterType, parameter.Name, parameter.IsOut, parameter.IsRef));
                        else
                            throw new Exception("unable to find parameter type");
                    }

                    if( savedMethod.SerializedGraph != null )
                        method.GraphDefinition = Graph.DevGraphDefinition.Load(savedMethod.SerializedGraph, project, method);

                    Methods.Add(method);
                }
                else
                    throw new Exception("Unable to load return type");
            }
        }
    }
}
