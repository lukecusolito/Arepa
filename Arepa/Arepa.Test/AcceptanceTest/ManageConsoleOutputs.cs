using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arepa.Parser;
using System.Diagnostics;
using Arepa.Test.Helper;
using System.Text.RegularExpressions;

namespace Arepa.Test.AcceptanceTest
{
    /// <summary>
    /// @Feature: Manage console outputs
    /// </summary>
    /// <remarks>
    /// As a Tester 
    /// I want useful information as output 
    /// so that I can use the information to make decisions
    /// </remarks>
    [TestClass]
    public class ManageConsoleOutputs
    {
        
        private ResourceFileManager resourceMan = null;
        private TestContext testContextInstance;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageConsoleOutputs()
        {
            resourceMan = new ResourceFileManager();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// @Scenario: Detect if starting message is displayed at the beginning of messages
        /// </summary>
        /// <remarks>
        /// Given a call to Arepa
        /// When the application is executed
        /// Then the first message displayed should be the arepa version and the starting time
        /// </remarks>
        [TestMethod]
        public void DetectIfStartingMessageIsDisplayedAtTheBeginningOfMessages()
        {
            //Given (Arrange)
            Process proc = new Process();
            proc.StartInfo.FileName = TestEnvironment.ConsolePath + "Arepa.exe";
            proc.StartInfo.Arguments = null;

            //When (Act)
            
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = TestEnvironment.ConsolePath;
            proc.Start();
            proc.WaitForExit();

            //Then (Assert)
            string message = proc.StandardOutput.ReadLine();

            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringStartingMessage"),true));
            Assert.IsTrue(s.IsMatch(message),"The starting message is not displayed correctly as output");
        }

        /// <summary>
        /// @Scenario: Detect if ending message is displayed at the ending of messages
        /// </summary>
        /// remarks>
        /// Given a call to Arepa
        /// When the application is executed
        /// Then the last message displayed should be the arepa version, the ending time and the total execution time
        /// </remarks>
        [TestMethod]
        public void DetectIfEndingMessageIsDisplayedAtTheEndingOfMessages()
        {
            //Given (Arrange)
            Process proc = new Process();
            proc.StartInfo.FileName = TestEnvironment.ConsolePath + "Arepa.exe";
            proc.StartInfo.Arguments = null;

            //When (Act)
            
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = TestEnvironment.ConsolePath;
            proc.Start();
            proc.WaitForExit();

            //Then (Assert)
            bool read = true;
            string message = string.Empty;
            string tempMessage = string.Empty;
            while (read)
            {
                tempMessage = proc.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(tempMessage))
                    message = tempMessage;
                else
                    read = false;
            }

            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringFinishingMessage"),true));
            Assert.IsTrue(s.IsMatch(message),"The ending message is not displayed correctly as output");
        }


        /// <summary>
        /// @Scenario: Display a message indicating no errors or suggestion
        /// </summary>
        /// <remarks>
        /// Given a valid documentation file
        /// And a valid test report
        /// When the Application is run
        /// Then a message indicating no error or suggestions should be displayed
        /// </remarks>
        [TestMethod]
        public void DisplayAMessageIndicatingNoErrorsOrSuggestion()
        {
            //Given (Arrange)
            Process proc = new Process();
            string projectDir = "\"" + TestEnvironment.MockPath + @"HtmlTestReport\ReportProjects\ArepaMocks.TestValidTestReport""";
            string testDir = "\"" + TestEnvironment.MockPath + @"HtmlTestReport\ReportProjects\ArepaMocks.TestValidTestReport\TestResults""";
            proc.StartInfo.Arguments = string.Format("-ProjectDir {0} -TestDir {1}", projectDir, testDir);

            //When (Act)
            proc.StartInfo.FileName = TestEnvironment.ConsolePath + "Arepa.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = TestEnvironment.ConsolePath;
            proc.Start();
            proc.WaitForExit();

            //Then (Assert)
            string errorMessage = "The message indicating no error or suggestions is not displayed on screen before the report name message";
            string message = proc.StandardOutput.ReadToEnd();
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringTemplateInformation"),false));
            Assert.IsTrue(s.IsMatch(message), errorMessage);
            s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringInformationNoErrorOrSuggestion"), false));
            Assert.IsTrue(s.IsMatch(message), errorMessage);
            
        }
    }
}
