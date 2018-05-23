using ArepaRunner.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ArepaRunner.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextOutput output;
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            mainViewModel = new MainViewModel();

            InitializeComponent();
            output =  new TextOutput();
            this.DataContext = mainViewModel;

            this.txt_Output.ScrollToEnd();
        }

        private void txt_Output_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txt_Output.ScrollToEnd();
        }
    }
}
