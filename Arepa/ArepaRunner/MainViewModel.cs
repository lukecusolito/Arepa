using ArepaRunner.Utilities;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ArepaRunner
{
    public class MainViewModel : ViewModelBase
    {
        private bool isRunning;

        private static App app;

        public ICommand SelectTestProjFileCommand { get; private set; }

        public ICommand SelectTestProjDirCommand { get; private set; }

        public ICommand SelectOutputDirCommand { get; private set; }

        public ICommand SelectTestResultsDirCommand { get; private set; }

        public ICommand SelectAssemblyFileCommand { get; private set; }

        public ICommand RunTestsCommand { get; private set; }

        public ICommand OpenReportsDirCommand { get; private set; }

        public MainViewModel()
        {
            app = System.Windows.Application.Current as App;
            isRunning = false;
            SelectTestProjFileCommand = new DelegateCommand(SelectTestProjFile, () => { return true; });
            SelectTestProjDirCommand = new DelegateCommand(SelectTestProjDir, () => { return true; });
            SelectOutputDirCommand = new DelegateCommand(SelectOutputDir, () => { return true; });
            SelectTestResultsDirCommand = new DelegateCommand(SelectTestResultsDir, () => { return true; });
            SelectAssemblyFileCommand = new DelegateCommand(SelectAssemblyFile, () => { return true; });
            OpenReportsDirCommand = new DelegateCommand(OpenReportsFolder, () => { return true; });
            RunTestsCommand = new DelegateCommand(RunTests, () => { return true; });
        }

        private void OpenReportsFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string dir = Path.GetDirectoryName(path) + "\\Reports";
            Directory.CreateDirectory(dir);
            Process.Start(@dir);
        }

        public bool CanExecuteRunTestsCommand
        {
            get
            {
                return Directory.Exists(ProjectDir) &&
                    Directory.Exists(OutputDir) &&
                    Directory.Exists(TestResultsDir) &&
                    File.Exists(ProjectFile) &&
                    File.Exists(AssemblyFile) &&
                    !IsRunning;
            }
        }

        private async void RunTests()
        {
            IsRunning = true;
            await Task.Run(() => Runner.Start(app.Args, (x) => { OutputText = outputText + x; }));
            IsRunning = false;
        }

        private void SelectTestProjFile()
        {
            GenericFileSelectionWithPropertyAssignment(FileType.ProjectFile, (x) => ProjectFile = x);
        }

        private void SelectTestProjDir()
        {
            GenericFolderSelectionWithPropertyAssignment((x) => ProjectDir = x);
        }

        private void SelectTestResultsDir()
        {
            GenericFolderSelectionWithPropertyAssignment((x) => TestResultsDir = x);
        }

        private void SelectOutputDir()
        {
            GenericFolderSelectionWithPropertyAssignment((x) => OutputDir = x);
        }

        private void SelectAssemblyFile()
        {
            GenericFileSelectionWithPropertyAssignment(FileType.DllFile, (x) => AssemblyFile = x);
        }

        private void GenericFolderSelectionWithPropertyAssignment(Action<string> onSuccess)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    onSuccess(fbd.SelectedPath);
                }
            }
        }

        private void GenericFileSelectionWithPropertyAssignment(FileType fileType, Action<string> onSuccess)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.DefaultExt = ArgumentHelper.GetFileExtension(fileType);
                ofd.Filter = ArgumentHelper.GetFileFilter(fileType);

                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    onSuccess(ofd.FileName);
                }
            }
        }

        public string ProjectFile
        {
            get
            {
                return app.Args.ProjectFile;
            }
            set
            {
                app.Args.ProjectFile = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        public string ProjectDir
        {
            get
            {
                return app.Args.ProjectDir;
            }
            set
            {
                app.Args.ProjectDir = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        public string TestResultsDir
        {
            get
            {
                return app.Args.TestResultsDir;
            }
            set
            {
                app.Args.TestResultsDir = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        public string OutputDir
        {
            get
            {
                return app.Args.OutputDir;
            }
            set
            {
                app.Args.OutputDir = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        public string AssemblyFile
        {
            get
            {
                return app.Args.AssemblyFile;
            }
            set
            {
                app.Args.AssemblyFile = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        public string TestCategory
        {
            get
            {
                return app.Args.TestCategory;
            }
            set
            {
                app.Args.TestCategory = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteRunTestsCommand");
            }
        }

        private string outputText;
        public string OutputText
        {
            get
            {
                return outputText;
            }
            set
            {
                outputText = value;
                OnPropertyChanged();
            }
        }
    }
}
