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
using DevEngine.Core.Evaluator;
using System.Threading.Tasks;

namespace DevEngine.FakeTypes.Project
{
    public class DevProject : IDevProject
    {
        public string Name { get; private set; }

        private IRealTypesProviderService RealTypesProviderService { get; }

        public IDevClassCollection Classes { get; } = new DevClassCollection();

        public string? Folder { get; private set; }

        public IDevType ExecType => DevExecType.ExecType;

        public readonly Func<string, string?> FileProvider;

        public DevProject(string name, IRealTypesProviderService realTypesProviderService, Func<string, string?>? fileProvider = null)
        {
            Name = name;
            RealTypesProviderService = realTypesProviderService;

            FileProvider = fileProvider ?? (file =>
            {
                file = Path.Combine(Folder, file);
                if (File.Exists(file))
                    return File.ReadAllText(file);
                return null;
            });
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

        public void Save(string folder, bool updateProjectFolder)
        {
            if (updateProjectFolder)
                Folder = folder;

            if (Directory.Exists(folder + "_backup"))
                Directory.Delete(folder + "_backup", true);

            // create a backup of the folder if it already exists, in case something goes wrong
            if (Directory.Exists(folder))
                Directory.Move(folder, folder + "_backup");

            Directory.CreateDirectory(folder);

            var projectContent = JsonSerializer.Serialize(new DevProjectSerializedContent(Name, Classes.Values.ToDictionary(x => x.Name.FullNameWithNamespace, x => Path.Combine(x.Folder, x.Name + ".json"))));
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
            Folder = folder;

            var projectFile = "project.json";

            var fileContent = FileProvider(projectFile);
            if (fileContent == null)
                throw new Exception("Project file not found:" + projectFile);

            var serializedContent = JsonSerializer.Deserialize<DevProjectSerializedContent>(fileContent);
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

            foreach (var classToLoad in Classes)
            {
                if (classToLoad.Value is DevClass devClass)
                    devClass.LoadPropertiesAfterPreload(this);
            }

            foreach (var classToLoad in Classes)
            {
                if (classToLoad.Value is DevClass devClass)
                    devClass.LoadMethodsAfterPreload(this);
            }
        }

        /// <summary>
        /// Used to load the very basic of a class, aka just no method or properties, just the class itself
        /// A <seealso cref="DevClass.Preload(IDevProject, string)"/> might also call this <seealso cref="PreloadClass(DevClassName)"/> if it needs to preload a parent class
        /// </summary>
        /// <param name=""></param>
        internal IDevClass PreloadClass(DevClassName devClass, string file, DevProjectSerializedContent devProjectSerializedContent)
        {
            if (Classes.TryGetValue(devClass, out var preloadedClass))
                return preloadedClass;

            var c = Classes[devClass] = DevClass.Preload(this, file, devProjectSerializedContent);

            return c;
        }

        internal IDevClass PreloadClass(DevClassName devClass, DevProjectSerializedContent devProjectSerializedContent)
        {
            return PreloadClass(devClass, devProjectSerializedContent.Classes[devClass.FullNameWithNamespace], devProjectSerializedContent);
        }

        internal IDevType PreloadClass(SavedTypeName savedClassName, DevProjectSerializedContent devProjectSerializedContent)
        {
            if (savedClassName.IsNetClass && savedClassName.FullNetClassName != null)
            {
                var type = Type.GetType(savedClassName.FullNetClassName);
                if (type != null)
                    return GetRealType(type); // could crash here
                throw new Exception("Unable to find net class:" + savedClassName.FullNetClassName);
            }
            else if (savedClassName.IsDevClass && savedClassName.FullDevClassName != null)
                return PreloadClass(savedClassName.FullDevClassName, devProjectSerializedContent);
            else
                throw new Exception("Unable to preload class from savedClassName");
        }

        internal IDevType GetTypeFromSavedClassName(SavedTypeName savedClassName)
        {
            if (savedClassName.IsNetClass && savedClassName.FullNetClassName != null)
            {
                var type = Type.GetType(savedClassName.FullNetClassName);
                if (type != null)
                    return (IDevClass)GetRealType(type); // could crash here
                throw new Exception("Unable to find net class:" + savedClassName.FullNetClassName);
            }
            else if (savedClassName.IsDevClass && savedClassName.FullDevClassName != null)
            {
                if (Classes.TryGetValue(savedClassName.FullDevClassName, out var devClass))
                    return devClass;
                throw new Exception("Unable to find devClass in preloaded classes:" + savedClassName.FullDevClassName.FullNameWithNamespace);
            }
            else
                throw new Exception("Unable to preload class from savedClassName");
        }

        #endregion

        #region RenameClass

        public void RenameClass(string oldFullNameWithNamespace, string newFullNameWithNamespace)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RunAsConsole

        public Task RunAsConsole(IDevGraphEvaluator evaluator, IConsoleLogger? consoleLogger)
        {
            return Task.Run(() =>
            {
                var compiler = evaluator.GetCompiler(this);

                compiler.CompileProject(Path.Combine(Folder ?? throw new Exception("Must save before compilation"), "compile"));

                var loadContext = new System.Runtime.Loader.AssemblyLoadContext("Debugger", true);
                try
                {
                    var path = Path.GetFullPath(Path.Combine(Folder, "compile", "bin", "x64", "Debug", "net6.0", Name + ".dll"));
                    var assembly = loadContext.LoadFromAssemblyPath(path);

                    var main = assembly.ExportedTypes.Select(x => x.GetMethod("Main")).First(x => x != null);

                    try
                    {
                        consoleLogger?.StartLogging();

                        main.Invoke(null, null);
                    }
                    finally
                    {
                        consoleLogger?.StopLogging();
                    }
                }
                finally
                {
                    loadContext.Unload();
                }
            });
        }

        #endregion
    }
}
