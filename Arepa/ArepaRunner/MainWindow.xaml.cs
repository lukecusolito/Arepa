using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ArepaRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App app;
        private TextOutput output;
        public MainWindow()
        {
            app = (Application.Current as App);

            InitializeComponent();
            output =  new TextOutput();
            this.DataContext = output;

            LoadInputBoxValues();
            this.txt_Output.ScrollToEnd();
        }

        public void AppendOutputText(string text)
        {
            this.txt_Output.AppendText(text);
        }

        private void btn_TestProjectDir_Click(object sender, RoutedEventArgs e)
        {
            var result = app.Args.SelectNewDirectory();
            if (result != string.Empty)
            {
                app.Args.ProjectDir = result;
                LoadInputBoxValues();
            }
        }

        private void btn_TestProjFile_Click(object sender, RoutedEventArgs e)
        {
            var result = app.Args.SelectNewFile(FileType.ProjectFile);
            if (result != string.Empty)
            {
                app.Args.ProjectFile = result;
                LoadInputBoxValues();
            }
        }

        private void btn_OutputDir_Click(object sender, RoutedEventArgs e)
        {
            var result = app.Args.SelectNewDirectory();
            if (result != string.Empty)
            {
                app.Args.OutputDir = result;
                LoadInputBoxValues();
            }
        }

        private void btn_TestResultsLocation_Click(object sender, RoutedEventArgs e)
        {
            var result = app.Args.SelectNewDirectory();
            if (result != string.Empty)
            {
                app.Args.TestResultsDir = result;
                LoadInputBoxValues();
            }
        }

        private void btn_TestAssembly_Click(object sender, RoutedEventArgs e)
        {
            var result = app.Args.SelectNewFile(FileType.DllFile);
            if (result != string.Empty)
            {
                app.Args.AssemblyFile = result;
                LoadInputBoxValues();
            }
        }

        private async void btn_Run_Click(object sender, RoutedEventArgs e)
        {
            app.Args.TestCategory = txt_TestCategory.Text;

            ToggleButtonsActive(false);
            await Task.Run(() => Runner.Start(app.Args, output));
            ToggleButtonsActive(true);
        }

        private void btn_Reports_Click(object sender, RoutedEventArgs e)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string dir = Path.GetDirectoryName(path) + "\\Reports";
            Directory.CreateDirectory(dir);
            Process.Start(@dir);
        }

        private void txt_Output_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txt_Output.ScrollToEnd();
        }

        #region Helper Methods
        private void LoadInputBoxValues()
        {
            this.txt_TestProjectDir.Text = app.Args.ProjectDir;
            this.txt_TestProjectFile.Text = app.Args.ProjectFile;
            this.txt_OutputDir.Text = app.Args.OutputDir;
            this.txt_TestResultLocation.Text = app.Args.TestResultsDir;
            this.txt_TestAssembly.Text = app.Args.AssemblyFile;
        }

        private void ToggleButtonsActive(bool active)
        {
            this.btn_TestProjectDir.IsEnabled = active;
            this.btn_OutputDir.IsEnabled = active;
            this.btn_Run.IsEnabled = active;
            this.btn_TestAssembly.IsEnabled = active;
            this.btn_TestProjFile.IsEnabled = active;
            this.btn_TestResultsLocation.IsEnabled = active;
        }
        #endregion
    }
}
