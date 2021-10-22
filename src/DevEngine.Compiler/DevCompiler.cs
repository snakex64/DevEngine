using DevEngine.Core.Class;
using DevEngine.Core.Method;
using DevEngine.Core.Project;
using System.Text;

namespace DevEngine.Compiler
{
    public class DevCompiler
    {
        private IDevProject Project { get; }

        public DevCompiler(IDevProject project)
        {
            Project = project;
        }

        public void CompileProject()
        {
            var tempFolder = Path.GetTempPath();

            Directory.CreateDirectory(tempFolder);

            foreach (var classToCompile in Project.Classes)
                CompileClass(classToCompile.Value, tempFolder);
        }

        #region CompileClass

        private void CompileClass(IDevClass devClass, string mainFolder)
        {
            if (devClass.IsRealType)
                throw new Exception("Cannot compile real types");

            var folder = Path.Combine(mainFolder, devClass.Folder);
            var file = Path.Combine(folder, devClass.Name + ".cs");
            Directory.CreateDirectory(folder);

            var builder = new StringBuilder();

            AddClassTopBoilerPlate(devClass, builder, out var indentation);

            AddProperties(devClass, builder, ref indentation);

            AddMethods(devClass, builder, ref indentation);

            AddClassBottomBoilerPlate(devClass, builder, indentation);


            var content = builder.ToString();
            File.WriteAllText(file, content);
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
            AddMethodHeader(devMethod, builder, ref indentation);

            builder.AppendLine(GetTabs(indentation) + "throw new NotImplementedException();");

            indentation--;
            builder.AppendLine(GetTabs(indentation) + "}");
        }

        #endregion


        #region AddMethodHeader

        private void AddMethodHeader(IDevMethod devMethod, StringBuilder builder, ref int indentation)
        {
            builder.Append(GetTabs(indentation) + devMethod.Visibility.ToString().ToLower() + " ");

            builder.Append(devMethod.ReturnType.TypeNamespaceAndName + " ");

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