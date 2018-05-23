using System;
using System.Diagnostics;

namespace ArepaRunner
{
    public class Runner
    {
        private static Action<string> _logInformation;
        public static void Start(ProgramArguments args, Action<string> logInformation)
        {
            _logInformation = logInformation;

            Build(args.MSBuildLocation, args.ProjectFile);
            Test(args.OutputDir, args.AssemblyFile, args.MSTestLocation, args.TestSettings);
            Report(args.ProjectDir, args.TestResultsDir, args.TestCategory);
        }

        private static void Build(string msBuildLocation, string testProjectLocation)
        {
            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "\"" + msBuildLocation + "\"";
                p.StartInfo.Arguments =  "\"" + testProjectLocation + "\"" + " /t:Clean;Rebuild";
                p.OutputDataReceived += new DataReceivedEventHandler(SortOutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(SortOutputHandler);

                p.Start();
                p.BeginOutputReadLine();
                
                p.WaitForExit();
            }
        }

        private static void Test(string testDir, string testAssemblyLocation, string msTestLocation, string testSettingsLocation)
        {
            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = testDir;
                p.StartInfo.FileName = "\"" + msTestLocation + "\"";
                p.StartInfo.Arguments = "/testcontainer:" + "\"" + testAssemblyLocation + "\"" + " /testsettings:" + "\"" + testSettingsLocation + "\"";
                p.OutputDataReceived += new DataReceivedEventHandler(SortOutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(SortOutputHandler);

                p.Start();
                p.BeginOutputReadLine();

                p.WaitForExit();
            }
        }
        private static void Report(string projectLocation, string testResultsLocation, string testCategoryArg)
        {
            string testCategory = string.Empty;

            if(!string.IsNullOrWhiteSpace(testCategoryArg))
            {
                testCategory = " -TESTCATEGORY \"" + testCategoryArg + "\"";
            }

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "\"" + "Arepa.exe" + "\"";
                p.StartInfo.Arguments = "-PROJECTDIR \"" + projectLocation + "\" -TESTDIR \"" + testResultsLocation + "\"" + testCategory;
                p.OutputDataReceived += new DataReceivedEventHandler(SortOutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(SortOutputHandler);

                p.Start();
                p.BeginOutputReadLine();

                p.WaitForExit();
            }
        }
        private static void SortOutputHandler(object sendingProcess, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            if(!String.IsNullOrEmpty(e.Data))
            {
                _logInformation(e.Data + Environment.NewLine);
            }
        }
    }
}
