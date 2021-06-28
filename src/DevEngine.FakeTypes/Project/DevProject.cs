using DevEngine.FakeTypes.Class;
using DevEngine.Core;
using DevEngine.Core.Class;
using DevEngine.Core.Project;
using DevEngine.RealTypes;
using System;
using System.Collections.Generic;
using System.Text;
using DevEngine.Core.Graph;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace DevEngine.FakeTypes.Project
{
    public class DevProject : IDevProject
    {
        public string Name { get; private set; }

        private IRealTypesProviderService RealTypesProviderService { get; }

        public IDevClassCollection Classes { get; } = new DevClassCollection();

        public DevProject(string name, IRealTypesProviderService realTypesProviderService)
        {
            Name = name;
            RealTypesProviderService = realTypesProviderService;
        }

        public IDevType GetRealType(Type type)
        {
            return RealTypesProviderService.GetDevType(this, type);
        }

        public IDevType GetRealType<T>()
        {
            return GetRealType(typeof(T));
        }

        public IDevType GetVoidType()
        {
            return GetRealType(typeof(void));
        }

        public IDevGraphNodeParameter CreateGraphNodeParameter(string name, IDevType devType, bool isInput, IDevGraphNode owner)
        {
            return new Graph.DevGraphNodeParameter(isInput, devType, name, owner);
        }

        #region Save

        public void Save(string folder)
        {
            if (Directory.Exists(folder + "_backup"))
                Directory.Delete(folder + "_backup", true);

            // create a backup of the folder if it already exists, in case something goes wrong
            if (Directory.Exists(folder))
                Directory.Move(folder, folder + "_backup");

            Directory.CreateDirectory(folder);

            var projectContent = JsonSerializer.Serialize(new DevProjectSerializedContent(Name, Classes.Values.ToDictionary(x => x.Name.FullNameWithNamespace, x => Path.Combine(x.Folder, x.Name.Name))));
            File.WriteAllText(Path.Combine(folder, "project.json"), projectContent);

            SaveClasses(folder);
        }

        private void SaveClasses(string folder)
        {
            foreach (var classToSave in Classes)
            {
                if (classToSave.Value.ShouldBeSaved)
                    classToSave.Value.Save(Path.Combine(folder, classToSave.Value.Folder, classToSave.Value.Name + ".json"));
            }
        }

        #endregion

        #region Load

        public void Load(string folder)
        {
            var projectFile = Path.Combine(folder, "project.json");
            if (!File.Exists(projectFile))
                throw new Exception("Project file not found:" + projectFile);

            var serializedContent = JsonSerializer.Deserialize<DevProjectSerializedContent>(File.ReadAllText(projectFile));
            if (serializedContent == null)
                throw new Exception("Unable to deserialize project file:" + projectFile);

            Name = serializedContent.Name;

            Classes.Clear();
            foreach (var classToLoad in serializedContent.Classes)
            {
                var devClassName = new DevClassName(classToLoad.Key);
                // classes might load parent classes, so we have to check if it's already loaded
                if (Classes.ContainsKey(devClassName))
                    continue;

                var preloadedClass = PreloadClass(devClassName, classToLoad.Value, serializedContent);
                Classes[devClassName] = preloadedClass;
            }



        }

        /// <summary>
        /// Used to load the very basic of a class, aka just no method or properties, just the class itself
        /// A <seealso cref="DevClass.Preload(IDevProject, string)"/> might also call this <seealso cref="PreloadClass(DevClassName)"/> if it needs to preload a parent class
        /// </summary>
        /// <param name=""></param>
        internal IDevClass PreloadClass(DevClassName devClass, string file, DevProjectSerializedContent serializedContent)
        {
            if (Classes.TryGetValue(devClass, out var preloadedClass))
                return preloadedClass;

            return DevClass.Preload(this, file, serializedContent);
        }
        internal IDevClass PreloadClass(DevClassName devClass, DevProjectSerializedContent devProjectSerializedContent)
        {
            return PreloadClass(devClass, devProjectSerializedContent.Classes[devClass.FullNameWithNamespace], devProjectSerializedContent);
        }

        #endregion
    }
}
