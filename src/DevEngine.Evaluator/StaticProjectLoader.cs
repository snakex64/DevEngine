using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Evaluator
{
    public static class StaticProjectLoader
    {
        public static IDevProject Load(Assembly assembly, string projectName)
        {
            var project = new FakeTypes.Project.DevProject(projectName, new RealTypes.RealTypesProviderService(), file =>
            {
                using var stream = assembly.GetManifestResourceStream( projectName + "._project." + file.Replace("\\", ".").Replace("/", "."));
                var names = assembly.GetManifestResourceNames();
                if (stream == null)
                    return null;

                using var streamReader = new StreamReader(stream);

                return streamReader.ReadToEnd();
            });

            project.Load("_project");

            return project;
        }
    }
}
