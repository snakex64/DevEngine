using DevEngine.Core.Class;
using DevEngine.Core.Project;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevEngine.Core
{

    /// <summary>
    /// Used to save the name of a type and where it's from ( nuget, .dll, existing .net type or from the current DevEngine project )
    /// </summary>
    public class SavedTypeName
    {
        public SavedTypeName() { }

        public SavedTypeName(IDevType devType)
        {
            if (devType.IsRealType)
            {
                IsNetClass = true;
                FullNetClassName = devType.TypeNamespaceAndName;
            }
            else
            {
                IsDevClass = true;
                FullDevClassName = devType.TypeNamespaceAndName;
            }
        }
        public DevClassName? FullDevClassName { get; set; }

        public bool IsDevClass { get; set; }

        public bool IsNetClass { get; set; }

        public string? FullNetClassName { get; set; }

        public bool TryGetDevType(IDevProject project, [MaybeNullWhen(false)] out IDevType devType)
        {
            if (IsNetClass)
            {
                var type = Type.GetType(FullNetClassName ?? throw new Exception("FullNetClassName shouldn't be null here"));
                if (type == null)
                {
                    devType = null;
                    return false;
                }

                devType = project.GetRealType(type);
                return true;
            }

            if (IsDevClass)
            {
                if (FullDevClassName == null)
                    throw new Exception("FullDevClassName shouldn't be null here");

                if (FullDevClassName.FullNameWithNamespace == "System.Exec")
                {
                    devType = DevExecType.ExecType;
                    return true;
                }

                var classFound = project.Classes.Where(x => x.Key.FullNameWithNamespace == FullDevClassName.FullNameWithNamespace).Select(x => x.Value).FirstOrDefault();

                if (classFound == null)
                {
                    devType = null;

                    return false;
                }

                devType = classFound;
                return true;
            }

            throw new Exception("IsDevClass or IsNetClass, at least one should be set");
        }
    }
}
