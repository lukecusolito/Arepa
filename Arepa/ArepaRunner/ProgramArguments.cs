using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ArepaRunner
{
    public class ProgramArguments
    {
        private const string projectDirArgument = "-PROJECTDIR";
        private const string outputDirArgument = "-OUTPUTDIR";
        private const string assemblyNameAttribute = "AssemblyName";

        public string ProjectDir { get; set; }
        public string OutputDir { get; set; }
        public string ProjectFile { get; set; }
        public string AssemblyFile { get; set; }
        public string TestResultsDir { get; set; }
        public string MSBuildLocation { get; set; }
        public string MSTestLocation { get; set; }
        public string TestCategory { get; set; }
        public string TestSettings { get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string dir = Path.GetDirectoryName(path);

                return dir + "\\testsettings.testsettings";
            }
        }

        public ProgramArguments(string[] args)
        {
            ParseArguments(args);
            LoadAppSettings();

            ProjectDir = PathRemoveBackslash(ProjectDir);
            OutputDir = PathRemoveBackslash(OutputDir);
            ProjectFile = PathRemoveBackslash(ProjectFile);
            AssemblyFile = PathRemoveBackslash(AssemblyFile);
            TestResultsDir = PathRemoveBackslash(TestResultsDir);
            MSBuildLocation = PathRemoveBackslash(MSBuildLocation);
            MSTestLocation = PathRemoveBackslash(MSTestLocation);
        }

        public string SelectNewFile(FileType type)
        {
            string file = string.Empty;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.DefaultExt = GetFileExtension(type);
                ofd.Filter = GetFileFilter(type);

                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    file = ofd.FileName;
                }
            }
            return file;
        }

        public string SelectNewDirectory()
        {
            string dir = string.Empty;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    dir = fbd.SelectedPath;
                }
            }
            return dir;
        }

        #region Helper Methods
        private void LoadAppSettings()
        {
            MSBuildLocation = ConfigurationManager.AppSettings.Get("MSBuildLocation");
            MSTestLocation = ConfigurationManager.AppSettings.Get("MSTestLocation");
        }

        private void ParseArguments(string[] args)
        {
            if (args != null && args.Count() > 0)
            {
                int i = 0;
                foreach (string argument in args)
                {
                    switch (argument.ToUpperInvariant())
                    {
                        case projectDirArgument:
                            if (args.Length > i + 1)
                                ProjectDir = args[i + 1];
                            break;
                        case outputDirArgument:
                            if (args.Length > i + 1)
                            {
                                OutputDir = args[i + 1];
                            }
                            break;
                    }
                    i++;
                }

                this.ProjectFile = GetProjectFile(ProjectDir);

                AssemblyFile = PathAddBackslash(OutputDir)  + GetAssemblyName(ProjectFile);
                TestResultsDir = PathAddBackslash(OutputDir) + "TestResults";
            }
        }

        private string GetProjectFile(string projectDirectory)
        {
            return GetFileName(projectDirectory, FileType.ProjectFile);
        }
        private string GetAssemblyName(string projectFile)
        {
            XDocument data = XDocument.Load(projectFile);

            string assemblyName = data.Descendants().Where(e => e.Name.LocalName == assemblyNameAttribute).FirstOrDefault().Value;

            return assemblyName + ".dll";
        }

        private static string GetFileName(string directory, FileType type)
        {
            string fileName = string.Empty;
            if (Directory.Exists(directory))
            {
                // Process the list of files found in the directory.
                var dir = new DirectoryInfo(directory);
                //Get the files in directory 
                FileInfo fi = null;
                switch (type)
                {
                    case FileType.ProjectFile:
                        fi = (dir.GetFiles("*.csproj").OrderByDescending(f => f.LastWriteTime)).FirstOrDefault<FileInfo>();
                        break;
                }
                if (fi != null)
                    fileName = fi.FullName;
            }
            return fileName;
        }

        private string GetFileExtension(FileType type)
        {
            string ext = string.Empty;

            switch(type)
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
        
        private string GetFileFilter(FileType type)
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

        private string PathAddBackslash(string path)
        {
            // They're always one character but EndsWith is shorter than
            // array style access to last path character. Change this
            // if performance are a (measured) issue.
            string separator1 = Path.DirectorySeparatorChar.ToString();
            string separator2 = Path.AltDirectorySeparatorChar.ToString();

            // Trailing white spaces are always ignored but folders may have
            // leading spaces. It's unusual but it may happen. If it's an issue
            // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
            path = path.TrimEnd();

            // Argument is always a directory name then if there is one
            // of allowed separators then I have nothing to do.
            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            // If there is the "alt" separator then I add a trailing one.
            // Note that URI format (file://drive:\path\filename.ext) is
            // not supported in most .NET I/O functions then we don't support it
            // here too. If you have to then simply revert this check:
            // if (path.Contains(separator1))
            //     return path + separator1;
            //
            // return path + separator2;
            if (path.Contains(separator2))
                return path + separator2;

            // If there is not an "alt" separator I add a "normal" one.
            // It means path may be with normal one or it has not any separator
            // (for example if it's just a directory name). In this case I
            // default to normal as users expect.
            return path + separator1;
        }

        private string PathRemoveBackslash(string path)
        {
            // They're always one character but EndsWith is shorter than
            // array style access to last path character. Change this
            // if performance are a (measured) issue.
            char separator1 = Path.DirectorySeparatorChar;
            char separator2 = Path.AltDirectorySeparatorChar;

            // Trailing white spaces are always ignored but folders may have
            // leading spaces. It's unusual but it may happen. If it's an issue
            // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
            //path = path.TrimEnd();

            // Argument is always a directory name then if there is one
            // of allowed separators then I have nothing to do.
            //if (!path.EndsWith(separator1) || !path.EndsWith(separator2))
            //return path;

            // If there is the "alt" separator then I add a trailing one.
            // Note that URI format (file://drive:\path\filename.ext) is
            // not supported in most .NET I/O functions then we don't support it
            // here too. If you have to then simply revert this check:
            // if (path.Contains(separator1))
            //     return path + separator1;
            //
            // return path + separator2;
            if (path.Contains(separator2.ToString()))
                return path.TrimEnd(separator2);

            // If there is not an "alt" separator I add a "normal" one.
            // It means path may be with normal one or it has not any separator
            // (for example if it's just a directory name). In this case I
            // default to normal as users expect.
            return path.TrimEnd(separator1);
        }
        #endregion
    }
}
