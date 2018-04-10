using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Arepa.Test.Helper;
using Arepa.Parser;
using System.Text.RegularExpressions;
using System.IO;

namespace Arepa.Test.AcceptanceTest
{
    /// <summary>
    /// @Feature: Manage Console Arguments
    /// </summary>
    /// <remarks>
    /// As a Tester
    /// I want to detect if I entered the right arguments and the right files are in my project
    /// So that I can be sure that I have all the file structure required to use Arepa
    /// </remarks>
    [TestClass]
    public class ManageConsoleArguments
    {
        private ResourceFileManager resourceMan = null;
        private TestContext testContextInstance;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageConsoleArguments()
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
        /// @Scenario: Detect if arguments entered in the console are invalid
        /// </summary>
        /// <remarks>
        /// Given an Arepa console application
        /// When there are not valid arguments
        /// Then an error should be raised
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "InvalidArguments.csv", "InvalidArguments#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\Console\\InvalidArguments.csv"), TestMethod]
        public void DetectIfArgumentsEnteredInTheConsoleAreValid()
        {
            //Given (Arrange)
            Process proc = new Process();
            proc.StartInfo.FileName = TestEnvironment.ConsolePath + "Arepa.exe";

            //When (Act)
            if (TestContext.DataRow["Argument"].ToString() == "null")
                proc.StartInfo.Arguments = null;
            else
                proc.StartInfo.Arguments = TestContext.DataRow["Argument"].ToString();

            //Then (Assert)
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = TestEnvironment.ConsolePath;
            proc.Start();
            proc.WaitForExit();

            // get output to testing console.
            string message = proc.StandardOutput.ReadToEnd();

            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringTemplateError"),false));
            Assert.IsTrue(s.IsMatch(message), message);
            s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString(TestContext.DataRow["ErrorMsg"].ToString()),false));
            Assert.IsTrue(s.IsMatch(message), message);
            
        }

        /// <summary>
        /// @Scenario: Accept valid arguments
        /// </summary>
        /// <remarks>
        /// Given a valid Project directory argument
        /// And a valid Test directory argument
        /// When a project directory is found
        /// And a project file is found
        /// And a test directory is found
        /// And a test file is found
        /// Then an object with the ProjectFileName should be returned
        /// And an object with the Test Report File Name should be returned
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ValidArguments.csv", "ValidArguments#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\Console\\ValidArguments.csv"), TestMethod]
        public void AcceptValidArguments()
        {
            //Given (Arrange)
            string[] args = TestContext.DataRow["Argument"].ToString().Split(' ');
            
            //Assign the right mock path to the arguments
            int i = 0;
            foreach (string argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-projectdir":
                        args[i + 1] = TestEnvironment.MockPath + args[i + 1];
                        //Update last write times on csproj files so correct one is selected for tests
                        File.SetLastWriteTime($"{args[i + 1]}\\{TestContext.DataRow["ProjectFileNameExpected"].ToString()}", DateTime.Now);
                        break;
                    case "-testdir":
                        args[i + 1] = TestEnvironment.MockPath + args[i + 1];
                        break;
                }
                i++;
            }

            //When (Act)
            ProgramArguments pArgs = new ProgramArguments(args);

            //Then (Assert)
            Assert.IsFalse(string.IsNullOrEmpty(pArgs.ProjectFileName));
            Assert.IsTrue(pArgs.ProjectFileName.EndsWith(TestContext.DataRow["ProjectFileNameExpected"].ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(pArgs.TestReportFile));
            Assert.IsTrue(pArgs.TestReportFile.EndsWith(TestContext.DataRow["TestFileNameExpected"].ToString()));
        }

        /// <summary>
        /// @Scenario: Detect if a proper error is raised when the project file doesn't exist
        /// </summary>
        /// <remarks>
        /// Given a projectDirectory
        /// When directory is not found
        /// OR not project file is found
        /// Then an a proper error should be raised
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "InvalidProjectDirectories.csv", "InvalidProjectDirectories#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\Console\\InvalidProjectDirectories.csv"), TestMethod]
        public void DetectIfProjectDirectoryOrFileExist()
        {
            //Given (Arrange)
            string projectDir = TestEnvironment.MockPath + TestContext.DataRow["ProjectDirectory"];

            //When (Act)
            FileManager fm = new FileManager();
            Message msg = null;
            fm.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            string projectFile = fm.GetProjectFileName(projectDir);

            //Then (Assert)
            Assert.IsTrue(string.IsNullOrEmpty(projectFile));
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString(TestContext.DataRow["ErrorMsg"].ToString()),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Detect if a proper error is raised when the test file doesn't exist
        /// </summary>
        /// <remarks>
        /// Given a testDirectory
        /// When directory is not found
        /// OR not test file is found
        /// Then an a proper error should be raised
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "InvalidTestDirectories.csv", "InvalidTestDirectories#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\Console\\InvalidTestDirectories.csv"), TestMethod]
        public void DetectIfTestDirectoryOrFileExist()
        {
            //Given (Arrange)
            string testDir = TestEnvironment.MockPath + TestContext.DataRow["TestDirectory"];

            //When (Act)
            FileManager fm = new FileManager();
            Message msg = null;
            fm.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            string testFile = fm.GetMSTestFileName(testDir);

            //Then (Assert)
            Assert.IsTrue(string.IsNullOrEmpty(testFile));
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString(TestContext.DataRow["ErrorMsg"].ToString()),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Get the project file name
        /// </summary>
        /// <remarks>
        /// Given a valid Project Directory
        /// When a valid project file exist on it
        /// Then the project file name should be returned
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ValidProjectDirectories.csv", "ValidProjectDirectories#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\Console\\ValidProjectDirectories.csv"), TestMethod]
        public void GetTheProjectFileName()
        {
            //Given (Arrange)
            string projectDir = TestEnvironment.MockPath + TestContext.DataRow["ProjectDirectory"];
            FileInfo fi = new FileInfo(projectDir + TestContext.DataRow["ProjectFileExpected"]);
            string projectFileNameExpected = fi.FullName;
           
            //When (Act)
            FileManager fm = new FileManager();
            string fileName = fm.GetProjectFileName(projectDir);
            
            //Then (Assert)
            Assert.AreEqual(projectFileNameExpected, fileName);
        }

        /// <summary>
        /// @Scenario: Detect if a documentation filename exists in a project file
        /// </summary>
        /// <remarks>
        /// Given a project file without documentation defined
        /// When a document name is not found in the project file
        /// Then an error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfADocumentationFileNameExistsInAProjectFile()
        {
            //Given (Arrange)
            string projectFileName = TestEnvironment.MockPath + "ArepaMocks.TestCProjectWithoutDocumentation.csproj";

            //When (Act)
            Project proj = new Project();
            FileParser parser = new FileParser();
            Message msg = null;
            parser.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            proj = parser.ParseProjectFile(proj, projectFileName);

            //Then (Assert)
            Assert.IsTrue(proj.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorDocumentationFileNotFoundInProjectFile"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Extract the documentation filename from the project file
        /// </summary>
        /// <remarks>
        /// Given a project file with documentation
        /// When the document name is found in the project file
        /// Then the document name should be returned
        /// </remarks>
        [TestMethod]
        public void ExtractTheDocumentationFileNameFromTheProjectFile()
        {
            //Given (Arrange)
            string projectFileName = TestEnvironment.MockPath + @"TestProjectDirectories\DirectoryWith1CProjectFile\ProjectCOne.csproj";
            string documentationFileNameExpected = Directory.GetParent(projectFileName).FullName + @"\bin\Debug\ArepaMocks.TestValidDocumentationWith4Scenario2Feature.XML";


            //When (Act)
            Project proj = new Project();
            FileParser parser = new FileParser();
            proj = parser.ParseProjectFile(proj, projectFileName);

            //Then (Assert)
            Assert.IsFalse(proj.Error);
            Assert.AreEqual(documentationFileNameExpected, proj.DocumentationFile);
        }     

    }
}
