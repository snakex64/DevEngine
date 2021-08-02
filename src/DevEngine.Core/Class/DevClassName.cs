using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public class DevClassName
    {
        public string FullNameWithNamespace { get; set; } = null!;

        // Name without namespace
        public string Name { get; set; } = null!;

        public string Namespace => FullNameWithNamespace[..^(Name.Length + 1)];

        public override string ToString() => FullNameWithNamespace;

        public override int GetHashCode() => FullNameWithNamespace.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DevClassName devClassName)
                return devClassName.FullNameWithNamespace == FullNameWithNamespace;

            return false;
        }

        public DevClassName()
        {
        }

        public DevClassName(string @namespace, string name)
        {
            FullNameWithNamespace = $"{@namespace}.{name}";
            Name = name;
        }

        public DevClassName(string nameWithNamespace)
        {
            var namespacesEnd = nameWithNamespace.LastIndexOf('.');
            if (namespacesEnd == -1)
                throw new ArgumentException($"Class name must contain at least one namespace: {nameWithNamespace}", nameof(nameWithNamespace));

            Name = nameWithNamespace[(namespacesEnd + 1)..];
            FullNameWithNamespace = nameWithNamespace;
        }

        public bool IsInNamespace(string @namespace, bool includeSubNamespaces = false)
        {
            if (includeSubNamespaces)
                return StartsWithNamespace(@namespace);
            return Namespace == @namespace;
        }

        public bool StartsWithNamespace(string @namespace)
        {
            return FullNameWithNamespace.StartsWith(@namespace + ".");
        }

        public static implicit operator DevClassName(string nameWithNamespace)
        {
            return new DevClassName(nameWithNamespace);
        }
    }
}
