using DevEngine.Core.Method;
using DevEngine.Core.Project;
using DevEngine.Core.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevEngine.Core.Class
{
    public interface IDevClass: IDevType
    {
        IDevType? BaseType { get; }

        // Represent the folder in the solution, not the absolute folder on the disk
        string Folder { get; }

        DevClassName Name { get; }

        IDevMethodCollection Methods { get; }

        IDevPropertyCollection Properties { get; }

        Visibility Visibility { get; }

        /// <summary>
        /// Called during project saving, indicate if this class should be saved on disk
        /// Usually only returns 'false' if it's a RealType
        /// </summary>
        /// <returns></returns>
        bool ShouldBeSaved { get; }
        
        void Save(string file);
    }
}
