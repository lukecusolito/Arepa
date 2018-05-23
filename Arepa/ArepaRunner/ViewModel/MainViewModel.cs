using Prism.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArepaRunner.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private bool isRunning;
        private string outputText;
        private static App app;

        #endregion

        #region Commands

        public ICommand RunTestsCommand { get; private set; }

        public ICommand OpenReportsDirCommand { get; private set; }

        #endregion

        public MainViewModel()
        {
            app = System.Windows.Application.Current as App;
            isRunning = false;
            OpenReportsDirCommand = new DelegateCommand(OpenReportsFolder, () => { return true; });
            RunTestsCommand = new DelegateCommand(RunTests, () => { return true; });
        }

        #region Properties

        public bool CanExecuteRunTestsCommand
        {
            get
            {
                return !IsRunning;
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

        public string Project
        {
            get
            {
                return app.Args.ProjectFile.Split('\\').Last();
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

        #endregion

        #region Private Methods

        private async void RunTests()
        {
            IsRunning = true;
            await Task.Run(() => Runner.Start(app.Args, (x) => { OutputText = outputText + x; }));
            IsRunning = false;
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

        #endregion
    }
}
