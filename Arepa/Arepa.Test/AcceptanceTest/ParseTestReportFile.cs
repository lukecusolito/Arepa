using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arepa.Parser;
using System.Text.RegularExpressions;
using Arepa.Test.Helper;
using System.IO;
using System.Collections.ObjectModel;

namespace Arepa.Test.AcceptanceTest
{
    /// <summary>
    /// @Feature: Parse Test Report File
    /// </summary>
    /// <remarks>
    /// As a tester 
    /// I want to parse the test report file 
    /// so that I can import test results
    /// </remarks>
    [TestClass]
    public class ParseTestReportFile
    {
        private ResourceFileManager resourceMan = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParseTestReportFile()
        {
            resourceMan = new ResourceFileManager();
        }

        private TestContext testContextInstance;

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
        /// @Scenario: Get the lastest MSTest File generated in a test directory
        /// </summary>
        /// <remarks>
        /// Given a test directory with some MSTest files
        /// When a mstest file is selected
        /// Then its creation date should be the greatest
        /// </remarks>
        [TestMethod]
        public void GetTheLastestMSTestFileGeneratedInATestDirectory()
        {
            //Given (Arrange)
            string testDirectory = TestEnvironment.MockPath + @"TestFiles\MSTest\FolderWithTestFiles\";
            string testFileNameExpected = Directory.GetParent(testDirectory).FullName + @"\MockMSTest1.trx";

            //When (Act)
            FileManager fm = new FileManager();
            string testFileName = fm.GetMSTestFileName(testDirectory);

            //Then (Assert)
            Assert.AreEqual(testFileNameExpected, testFileName);
        }

        /// <summary>
        /// @Scenario: Detect if the test report file exits
        /// </summary>
        /// <remarks>
        /// Given a file name
        /// When the application doesn't found the file
        /// Then an error should be returned
        /// </remarks>
        [TestMethod]
        public void DetectIfTheTestReportFileExits()
        {
            //Given (precondition)
            string testReportFile = "nonexistentfile.trx";

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();

            Message msg = null;
            configParser.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            projectTested = configParser.ParseMSTestFile(projectTested, testReportFile, string.Empty);


            //Then (Assert)
            Assert.IsTrue(projectTested.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorFileNotFound"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));

        }

        /// <summary>
        /// @Scenario: Read general test values
        /// </summary>
        /// <remarks>
        /// Given a MSTest file
        /// When the values are read
        /// Then the test project name should be returned
        /// And the description should be returned
        /// And the start time should be returned
        /// And the end time should be returned
        /// </remarks>
        [TestMethod]
        public void ReadGeneralTestValues()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\MSTestReport.trx";
            Project pResult = TestEnvironment.BuildMockProjectWithoutTest();

            //When (Act)           
            FileParser fp = new FileParser();
            pResult = fp.ParseMSTestFile(pResult, testReportFile, string.Empty);

            //Then (Assert)
            Assert.AreEqual(Convert.ToDateTime("2012-03-20T11:31:31.9837420+11:00"), pResult.StartTime);
            Assert.AreEqual(Convert.ToDateTime("2012-03-20T11:31:38.1425953+11:00"), pResult.FinishTime);
        }

        /// <summary>
        /// @Scenario: Read success values from the MSTest file
        /// </summary>
        /// <remarks>
        /// Given a MSTest file with some test scenarios passed
        /// When the values are read
        /// Then scenarios with success information should be returned
        /// </remarks>
        [TestMethod]
        public void ReadSuccessValuesFromTheMSTestFile()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\MSTestReport.trx";
            Project pResult = TestEnvironment.BuildMockProjectWithoutTest();

            //When (Act)           
            FileParser fp = new FileParser();
            pResult = fp.ParseMSTestFile(pResult, testReportFile, string.Empty);

            //Then (Assert)
            int totalScenariosPassed = 0;
            foreach (Feature f in pResult.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    if (s.Passed)
                        totalScenariosPassed++;
                }
            }
            Assert.AreEqual(4, totalScenariosPassed);
        }

        /// <summary>
        /// @Scenario: Read error details from the MSTest file
        /// </summary>
        /// <remarks>
        /// Given a MSTest file with some test scenarios failed
        /// When the values are read
        /// Then the error details per scenario failed should be returned
        /// </remarks>
        [TestMethod]
        public void ReadErrorDetailsFromTheMSTestFile()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\MSTestReport.trx";
            Project pResult = TestEnvironment.BuildMockProjectWithoutTest();

            //When (Act)          
            FileParser fp = new FileParser();
            pResult = fp.ParseMSTestFile(pResult, testReportFile, string.Empty);

            //Then (Assert)
            foreach (Feature f in pResult.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    if (!s.Passed)
                    {
                        switch (s.MemberName)
                        {
                            case "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario3":
                                Assert.AreEqual("Assert.IsTrue failed. This test didn&#39;t pass because is its intention", s.ErrorDescription);
                                break;
                            case "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario4":
                                Assert.AreEqual("NotExecuted", s.ErrorDescription);
                                break;
                        }
                    }
                }
            }       
        }

        /// <summary>
        /// @Scenario: Identify scenarios without test associated
        /// </summary>
        /// <remarks>
        /// Given a test report file without tests for some scenarios
        /// When the report is read
        /// Then the scenarios without test associated should be identified
        /// </remarks>
        [TestMethod]
        public void IdentifyScenariosWithoutTestAssociated()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\MSTestReport.trx";
            Project pResult = TestEnvironment.BuildMockProjectWithoutTest();
            Scenario scenarioWithoutTest = new Scenario();
            scenarioWithoutTest.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.ScenarioWithoutTest";
            pResult.Features.Where(x => x.MemberName == "ArepaMocks.TestValidTestReport.FeatureSuccess1").FirstOrDefault<Feature>().Scenarios.Add(scenarioWithoutTest);

            //When (Act)           
            FileParser fp = new FileParser();
            pResult = fp.ParseMSTestFile(pResult, testReportFile, string.Empty);

            //Then (Assert)
            foreach (Feature f in pResult.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    if (s.MemberName == "ArepaMocks.TestValidTestReport.FeatureSuccess1.ScenarioWithoutTest")
                        Assert.IsFalse(s.HasTest);
                    else
                        Assert.IsTrue(s.HasTest);
                }
            }
        }

        /// <summary>
        /// @Scenario: Raise an error if not test was found for at least one scenario
        /// </summary>
        /// <remarks>
        /// Given a test report file without test for any scenario
        /// When the report is read
        /// Then a proper error should be raised
        /// </remarks>
        [TestMethod]
        public void RaiseErrorIfNotTestFoundForAtLeastOneScenario()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\MSTestReport.trx";
            Project badProject = new Project();
            Feature f = new Feature();
            f.MemberName = "NameSpace.ClassName";
            Scenario s1 = new Scenario();
            s1.MemberName = "NameSpace.ClassName.MethodName";
            f.Scenarios = new Collection<Scenario> { s1 };
            badProject.Features = new Collection<Feature> { f };

            //When (Act)
            FileParser fp = new FileParser();
            Message msg = null;
            fp.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            badProject = fp.ParseMSTestFile(badProject, testReportFile, string.Empty);


            //Then (Assert)
            Assert.IsTrue(badProject.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorNoTestResultsFound"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));


        }

        // TODO: ReportDescription is depricated (Code commented in solution). Confirm and remove. Replaced already by app name and test category.
        /*
        /// <summary>
        /// @Scenario: Test project without test settings
        /// </summary>
        /// <remarks>
        /// Given a test project without test settings
        /// When the test result file is read
        /// Then the Name on test report should be populated
        /// And the Description on test report should be default description
        /// </remarks>
        [TestMethod]
        public void TestProjectWithoutTestSettings()
        {
            //Given (Arrange)
            string testReportFile = TestEnvironment.MockPath + @"TestFiles\MSTest\ArepaMocks.ProjectWithoutTestsettings.Tests.trx";

            //When (Act)
            Project p = new Project();
            FileParser fp = new FileParser();
            p = fp.ParseMSTestFile(p, testReportFile, string.Empty);

            //Then (Assert)
            Assert.IsFalse(string.IsNullOrEmpty(p.Name));
            Assert.AreEqual(resourceMan.Resources.GetString("StringReportDefaultDescription"), p.Description, "Project description is not replace by the default description on projects without testsettings");

        }
        */
    }
}
