using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArepaRunner.Utilities
{
    public class ArgumentHelper
    {
        public static string GetFileExtension(FileType type)
        {
            string ext = string.Empty;

            switch (type)
            {
                case (FileType.ProjectFile):
                    ext = ".csproj";
                    break;
                case (FileType.DllFile):
                    ext = ".dll";
                    break;
                default:
                    break;
            }

            return ext;
        }

        public static string GetFileFilter(FileType type)
        {
            string filter = string.Empty;

            switch (type)
            {
                case (FileType.ProjectFile):
                    filter = "Project files| *.csproj";
                    break;
                case (FileType.DllFile):
                    filter = "DLL file| *.dll";
                    break;
            }

            return filter;
        }
    }
}
