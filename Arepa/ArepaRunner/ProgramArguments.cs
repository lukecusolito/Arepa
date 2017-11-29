using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
                string path = Directory.GetCurrentDirectory();
                return path + "\\testsettings.testsettings";
            }
        }

        public ProgramArguments(string[] args)
        {
            ParseArguments(args);
            LoadAppSettings();
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
                AssemblyFile = OutputDir + "\\" + GetAssemblyName(ProjectFile);
                TestResultsDir = OutputDir + "\\TestResults";
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
        #endregion
    }
}
