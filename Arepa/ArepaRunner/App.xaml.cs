using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ArepaRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ProgramArguments args;
        public ProgramArguments Args { get { return args; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            args = new ProgramArguments(e.Args);

            base.OnStartup(e);
        }
    }
}
