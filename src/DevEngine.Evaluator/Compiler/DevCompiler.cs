using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DevEngine.Evaluator.Compiler
{
    public class DevCompiler : Core.Evaluator.IDevCompiler
    {
        private IDevProject Project { get; }

        public DevCompiler(IDevProject project)
        {
            Project = project;
        }

        public void CompileProject(string folder)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception)
                { }
            }

            Directory.CreateDirectory(folder);

            SaveDevProject(folder);

            CreateCsproj(folder);

            CreateStaticProjectLoader(folder);

            foreach (var classToCompile in Project.Classes)
                GenerateClass(classToCompile.Value, folder);

            var startInfo = new ProcessStartInfo("dotnet", "build")
            {
                WorkingDirectory = folder,
            };
            var process = Process.Start(startInfo) ?? throw new Exception("Couldn't start build process");

            process.WaitForExit();
        }

        #region CreateCsproj

        private void CreateCsproj(string folder)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            stringBuilder.AppendLine("\t<PropertyGroup>");
            stringBuilder.AppendLine("\t\t<TargetFramework>net6.0</TargetFramework>");
            stringBuilder.AppendLine("\t</PropertyGroup>");


            stringBuilder.AppendLine("\t<ItemGroup>");
            stringBuilder.AppendLine("\t\t<None Remove=\"_project/**\" />");
            stringBuilder.AppendLine("\t\t<EmbeddedResource Include=\"_project/**\" />");
            stringBuilder.AppendLine("\t</ItemGroup>");

            stringBuilder.AppendLine("\t<ItemGroup>");
            var assemblies = new[]
            {
                System.Reflection.Assembly.GetExecutingAssembly(),
                System.Reflection.Assembly.GetAssembly(typeof(FakeTypes.Project.DevProject)) ?? throw new Exception("Unable to get assembly for FakeTypes"),
                System.Reflection.Assembly.GetAssembly(typeof(Core.DevObject)) ?? throw new Exception("Unable to get Core assembly"),
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( x=> x.GetName().Name == "DevEngine.Standard") ?? throw new Exception("Uanble to get DevEngine.Standard")
            };
            foreach (var assembly in assemblies)
            {
                var location = assembly.Location.Replace("\\", "\\\\");
                stringBuilder.AppendLine("\t\t<Reference Include=\"" + assembly.GetName().Name + "\">");
                stringBuilder.AppendLine("\t\t\t<HintPath>" + location + "</HintPath>");
                stringBuilder.AppendLine("\t\t</Reference>");
            }
            stringBuilder.AppendLine("\t</ItemGroup>");

            stringBuilder.AppendLine("</Project>");

            var content = stringBuilder.ToString();

            File.WriteAllText(Path.Combine(folder, Project.Name + ".csproj"), content);
        }

        #endregion

        #region CreateStaticProjectLoader

        private void CreateStaticProjectLoader(string folder)
        {
            var builder = new StringBuilder();

            int indentation = 0;
            builder.AppendLine("using System;");
            builder.AppendLine("using DevEngine.Core.Project;");

            builder.AppendLine($"namespace StaticProjectLoaderNamespace;");

            builder.AppendLine($"public static class StaticProjectLoader");
            builder.AppendLine("{");
            indentation++;

            builder.AppendLine(GetTabs(indentation) + "private readonly static Lazy<IDevProject> Project_;");
            builder.AppendLine(GetTabs(indentation) + "public static IDevProject Project => Project_.Value;");

            builder.AppendLine(GetTabs(indentation) + "static StaticProjectLoader()");
            builder.AppendLine(GetTabs(indentation) + "{");
            indentation++;

            builder.AppendLine(GetTabs(indentation) + "var assembly = System.Reflection.Assembly.GetExecutingAssembly();");
            builder.AppendLine(GetTabs(indentation) + "Project_ = new Lazy<IDevProject>(() => DevEngine.Evaluator.StaticProjectLoader.Load(assembly, \"" + Project.Name + "\"));");

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}");

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}");

            var content = builder.ToString();
            File.WriteAllText(Path.Combine(folder, "StaticProjectLoader.cs"), content);
        }

        #endregion

        #region SaveDevProject

        private void SaveDevProject(string folder)
        {
            Project.Save(Path.Combine(folder, "_project"));
        }

        #endregion

        #region GenerateClass

        private void GenerateClass(IDevClass devClass, string mainFolder)
        {
            if (devClass.IsRealType)
                throw new Exception("Cannot compile real types");

            var folder = Path.Combine(mainFolder, devClass.Folder);
            var file = Path.Combine(folder, devClass.Name + ".cs");
            Directory.CreateDirectory(folder);

            var builder = new StringBuilder();

            AddClassTopBoilerPlate(devClass, builder, out var indentation);

            AddClassAndMethodsLazyLoading(devClass, builder, ref indentation);

            AddProperties(devClass, builder, ref indentation);

            AddMethods(devClass, builder, ref indentation);

            AddClassBottomBoilerPlate(devClass, builder, indentation);


            var content = builder.ToString();
            File.WriteAllText(file, content);
        }

        #endregion

        #region AddClassAndMethodsLazyLoading

        private void AddClassAndMethodsLazyLoading(IDevClass devClass, StringBuilder builder, ref int indentation)
        {
            builder.AppendLine(GetTabs(indentation) + $"static {devClass.Name.Name}()");
            builder.AppendLine(GetTabs(indentation) + "{");
            indentation++;

            builder.AppendLine(GetTabs(indentation) + "var project = StaticProjectLoaderNamespace.StaticProjectLoader.Project;");
            builder.AppendLine(GetTabs(indentation) + "var devClass = project.Classes[\"" + devClass.Name.FullNameWithNamespace + "\"];");


            foreach (var method in devClass.Methods)
            {
                builder.AppendLine(GetTabs(indentation) + "_DevMethod_" + method.Name + " = devClass.Methods.Single(x => x.Name == \"" + method.Name + "\");");
            }

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}");
        }

        #endregion

        #region AddMethods

        private void AddMethods(IDevClass devClass, StringBuilder builder, ref int indentation)
        {
            foreach (var method in devClass.Methods)
            {
                AddMethod(method, builder, ref indentation);
            }
        }

        #endregion

        #region AddMethod

        private void AddMethod(IDevMethod devMethod, StringBuilder builder, ref int indentation)
        {
            builder.AppendLine(GetTabs(indentation) + "private static DevEngine.Core.Method.IDevMethod _DevMethod_" + devMethod.Name + ";");

            AddMethodHeader(devMethod, builder, ref indentation);


            // add the inputs
            builder.AppendLine(GetTabs(indentation) + "var _inputs = new System.Collections.Generic.Dictionary<string, DevEngine.Core.DevObject>");
            builder.AppendLine(GetTabs(indentation) + "{");
            indentation++;

            foreach (var input in devMethod.Parameters.Where(x => !x.IsOut))
                builder.AppendLine(GetTabs(indentation) + "[ \"" + input.Name + "\" ] = " + input.Name + ",");

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "};");

            builder.AppendLine();
            var thisObj = "new DevEngine.Core.DevObject(_DevMethod_" + devMethod.Name + ".DeclaringType, " + (devMethod.IsStatic ? "null" : "this") + ")";
            builder.AppendLine(GetTabs(indentation) + "new DevEngine.Evaluator.DevGraphEvaluator().Evaluate(" + thisObj + ", _DevMethod_" + devMethod.Name + ", _inputs, out var _outputs);");

            // set the outputs
            foreach (var output in devMethod.Parameters.Where(x => x.IsOut))
                builder.AppendLine(GetTabs(indentation) + output.Name + " = (" + output.ParameterType.TypeNamespaceAndName + ")_outputs[ \"" + output.Name + "\" ].Value;");

            if (devMethod.ReturnType != Project.GetVoidType())
                builder.AppendLine(GetTabs(indentation) + "return (" + devMethod.ReturnType.TypeNamespaceAndName + ")_outputs[ \"return\" ].Value;");

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}");
        }

        #endregion

        #region AddMethodHeader

        private void AddMethodHeader(IDevMethod devMethod, StringBuilder builder, ref int indentation)
        {
            builder.Append(GetTabs(indentation) + devMethod.Visibility.ToString().ToLower() + " ");

            if (devMethod.IsStatic)
                builder.Append("static ");

            var returnType = devMethod.ReturnType == Project.GetVoidType() ? "void" : devMethod.ReturnType.TypeNamespaceAndName;
            builder.Append(returnType + " ");

            builder.Append(devMethod.Name + "(");

            var parameters = devMethod.Parameters.Select(x => (x.IsOut ? "out " : x.IsRef ? "ref " : "") + x.ParameterType.TypeNamespaceAndName + " " + x.Name);
            builder.Append(string.Join(", ", parameters));

            builder.AppendLine(")");

            builder.AppendLine(GetTabs(indentation) + "{");
            indentation++;
        }

        #endregion

        #region AddProperties

        private void AddProperties(IDevClass devClass, StringBuilder builder, ref int indentation)
        {
            foreach (var property in devClass.Properties.Values)
            {
                var biggestVisibility = (Core.Visibility)Math.Max((int)property.GetVisibility, (int)property.SetVisibility);
                builder.Append(GetTabs(indentation) + biggestVisibility.ToString().ToLower() + " ");

                builder.Append(property.PropertyType.TypeNamespaceAndName + " " + property.Name + " ");

                if (property.GetVisibility == property.SetVisibility)
                    builder.AppendLine("{ get; set; }");
                else if (property.GetVisibility < property.SetVisibility)
                    builder.AppendLine("{ " + property.GetVisibility.ToString().ToLower() + " get; set; }");
                else
                    builder.AppendLine("{ get;" + property.GetVisibility.ToString().ToLower() + " set; }");

                builder.AppendLine();
            }
        }

        #endregion

        #region AddClassBottomBoilerPlate

        private void AddClassBottomBoilerPlate(IDevClass devClass, StringBuilder builder, int indentation)
        {
            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}"); // close the class

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}"); // close the namespace
        }

        #endregion

        #region AddClassTopBoilerPlate

        private void AddClassTopBoilerPlate(IDevClass devClass, StringBuilder builder, out int indentation)
        {
            indentation = 0;
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Linq;");

            builder.AppendLine($"namespace {devClass.TypeNamespace}");
            builder.AppendLine("{");
            indentation++;

            builder.AppendLine($"{GetTabs(indentation)}public class {devClass.TypeName}");
            builder.AppendLine(GetTabs(indentation) + "{");
            indentation++;
        }

        #endregion

        #region GetTabs

        private string GetTabs(int indentation)
        {
            return new string('\t', indentation);
        }

        #endregion

    }
}