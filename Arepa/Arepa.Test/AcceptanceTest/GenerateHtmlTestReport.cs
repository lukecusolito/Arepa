using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arepa.Parser;
using Arepa.Test.Helper;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Arepa.Test.AcceptanceTest
{
    /// <summary>
    /// @Feature: Generate Html Test Report
    /// </summary>
    /// <remarks>
    /// As a StakeHolder 
    /// I want to have a portable testing report 
    /// so that I can have a human readable report about the status of test scenarios related with my requirements
    /// </remarks>
    [TestClass]
    public class GenerateHtmlTestReport
    {
        private ResourceFileManager resourceMan = null;

        /// <summary>
        /// Default contructor
        /// </summary>
        public GenerateHtmlTestReport()
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
        /// @Scenario: Assign values to the Scenario Template
        /// </summary>
        /// <remarks>
        /// Given a Scenario template with all keywords (case-insensitive)
        /// And a scenario with test
        /// When the template is read
        /// Then the template content should be replaced with the scenario values on memory
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ValidReportTemplates.csv", "ValidReportTemplates#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\HtmlTestReport\\ValidReportTemplates.csv"), TestMethod]
        public void AssignValuesToTheScenarioTemplate()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            string scenarioTemplate = TestEnvironment.MockPath + TestContext.DataRow["ScenarioTemplate"];
            string featureTemplate = TestEnvironment.MockPath + TestContext.DataRow["FeatureTemplate"];
            string testReportTemplate = TestEnvironment.MockPath + TestContext.DataRow["TestReportTemplate"];

            //When (Act)
            Report r = new Report(p);
            r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);

            //Then (Assert)
            Assert.IsFalse(p.Error);
            foreach (Feature f in p.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    Assert.IsNotNull(s.ReportContent, "Report content is not pupulated for scenario");
                    Assert.AreNotEqual(-1, s.ReportContent.IndexOf(s.Title),"Title on scenario template is not populated");
                    Assert.AreNotEqual(-1, s.ReportContent.IndexOf(s.Description), "Description on scenario template is not populated");
                    if (s.Passed)
                    {
                        Assert.AreNotEqual(-1, s.ReportContent.IndexOf(resourceMan.Resources.GetString("StringReportYes")), "Passed on scenario template is not populated");
                        Assert.AreEqual(-1, s.ReportContent.IndexOf("[ScenarioErrorDescription]"), "[ScenarioErrorDescription] on scenario template is not deleted from template for scenarios passed");

                    }
                    else
                    {
                        Assert.AreNotEqual(-1, s.ReportContent.IndexOf(resourceMan.Resources.GetString("StringReportNo")), "Passed on scenario template is not populated");
                        Assert.AreNotEqual(-1, s.ReportContent.IndexOf(s.ErrorDescription), "Error description on scenario template is not populated for scenarios not passed");
                    }
                }
            }
        }

        /// <summary>
        /// @Scenario: Detect if the Scenario template exists
        /// </summary>
        /// <remarks>
        /// Given a invalid scenario file name
        /// When the template is read
        /// Then a proper error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfTheScenarioTemplateExists()
        {
            //Given (Arrange)
            string invalidScenarioFileName = TestEnvironment.MockPath + @"InvalidFile.arepa";
            string validFeatureFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Feature.arepa";
            string validTestReportFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\TestReport.arepa";

            //When (Act)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            Report r = new Report(p);
            Message msg = null;
            r.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };
            string reportContent = r.PrepareReportContent(invalidScenarioFileName, validFeatureFileName, validTestReportFileName);

            //Then (Assert)
            Assert.IsTrue(string.IsNullOrEmpty(reportContent));
            Assert.IsTrue(p.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorReportTemplateNotFound"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Only scenarios with test whould be included on report
        /// </summary>
        /// <remarks>
        /// Given a project with scenarios without test associated
        /// When the templates are read
        /// Then these scenarios should not been reflected on the report
        /// </remarks>
        [TestMethod]
        public void OnlyScenariosWithTestWhouldBeIncludedOnReport()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithScenariosWithBothTestAndNoTest();
            string scenarioTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Scenario.arepa";
            string featureTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Feature.arepa";
            string testReportTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\TestReport.arepa";

            //When (Act)
            Report r = new Report(p);
            r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);

            //Then (Assert)
            Assert.IsFalse(p.Error);
            foreach (Feature f in p.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    if (s.HasTest)
                        Assert.IsNotNull(s.ReportContent);
                    else
                        Assert.IsNull(s.ReportContent,"Scenarios without test are populated with a report content");
                }
            }
        }

        /// <summary>
        /// @Scenario: Detect if the Feature template exists
        /// </summary>
        /// <remarks>
        /// Given a invalid feature file name
        /// When the template is read
        /// Then a proper error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfTheFeatureTemplateExists()
        {
            //Given (Arrange)
            string validScenarioFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Scenario.arepa";
            string invalidFeatureFileName = TestEnvironment.MockPath + @"InvalidFile.arepa";
            string validTestReportFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\TestReport.arepa";

            //When (Act)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            Report r = new Report(p);
            Message msg = null;
            r.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };
            string reportContent = r.PrepareReportContent(validScenarioFileName, invalidFeatureFileName, validTestReportFileName);

            //Then (Assert)
            Assert.IsTrue(string.IsNullOrEmpty(reportContent));
            Assert.IsTrue(p.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorReportTemplateNotFound"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Assign values to the Feature Template
        /// </summary>
        /// <remarks>
        /// Given a Feature template with all keywords (case-insensitive)
        /// And one feature with test
        /// When the template is read
        /// Then the template content should be replaced with the feature values
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ValidReportTemplates.csv", "ValidReportTemplates#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\HtmlTestReport\\ValidReportTemplates.csv"), TestMethod]
        public void AssignValuesToTheFeatureTemplate()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            string scenarioTemplate = TestEnvironment.MockPath + TestContext.DataRow["ScenarioTemplate"];
            string featureTemplate = TestEnvironment.MockPath + TestContext.DataRow["FeatureTemplate"];
            string testReportTemplate = TestEnvironment.MockPath + TestContext.DataRow["TestReportTemplate"];

            //When (Act)
            Report r = new Report(p);
            r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);

            //Then (Assert)
            Assert.IsFalse(p.Error);
            foreach (Feature f in p.Features)
            {
                Assert.IsNotNull(f.ReportContent, "Report content is not populated for feature");
                Assert.AreNotEqual(-1, f.ReportContent.IndexOf(f.Title), "Title on feature template is not populated");
                Assert.AreNotEqual(-1, f.ReportContent.IndexOf(f.UserStory), "User story on feature template is not populated");
                Assert.AreNotEqual(-1, f.ReportContent.IndexOf(f.Scenarios.Count.ToString()), "Total scenarios on feature template are not populated");
                Assert.AreNotEqual(-1, f.ReportContent.IndexOf(f.Scenarios.Where(x => x.Passed == true).ToList<Scenario>().Count.ToString()), "Total scenarios passed on feature template are not populated");
                int percentage =  Convert.ToInt32((f.Scenarios.Where(x => x.Passed == true).ToList<Scenario>().Count * 100) / f.Scenarios.Count);
                Assert.AreNotEqual(-1, f.ReportContent.IndexOf(percentage+"%"), "Success rate on feature template are not populated");
                
                if (featureTemplate.EndsWith("FeatureWithoutScenarioSection.arepa"))
                {
                    //Confirm if scenarios where not pupulated for a template without scenariosection
                    //Confirm if scenario seccion was populated
                    foreach (Scenario s in f.Scenarios)
                    {
                        Assert.AreEqual(-1, f.ReportContent.IndexOf(s.Title), "Scenarios are populated on feature template without [scenariosection]");
                    }
                }
                else
                {
                    //Confirm if scenario seccion was populated
                    foreach (Scenario s in f.Scenarios)
                    {
                        Assert.AreNotEqual(-1, f.ReportContent.IndexOf(s.Title), "Scenarios are not populated on feature template with [scenariosection]");
                    }
                }
            }
        }

        /// <summary>
        /// @Scenario: Detect if the test report template exists
        /// </summary>
        /// <remarks>
        /// Given a invalid test report file name
        /// When the template is read
        /// Then a proper error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfTheTestReportTemplateExists()
        {
            //Given (Arrange)
            string validScenarioFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Scenario.arepa";
            string validFeatureFileName = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Feature.arepa";
            string invalidTestReportFileName = TestEnvironment.MockPath + @"InvalidFile.arepa";

            //When (Act)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            Report r = new Report(p);
            Message msg = null;
            r.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };
            string reportContent = r.PrepareReportContent(validScenarioFileName, validFeatureFileName, invalidTestReportFileName);

            //Then (Assert)
            Assert.IsTrue(string.IsNullOrEmpty(reportContent));
            Assert.IsTrue(p.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(resourceMan.Resources.GetString("StringErrorReportTemplateNotFound"),true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        /// <summary>
        /// @Scenario: Assign values to the Test Report Template
        /// </summary>
        /// <remarks>
        /// Given a Test Report template with all keywords (case-insensitive)
        /// And one project with all its value
        /// When the template is read
        /// Then the template content should be replaced with the feature values
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ValidReportTemplates.csv", "ValidReportTemplates#csv", DataAccessMethod.Sequential), DeploymentItem("Sources\\HtmlTestReport\\ValidReportTemplates.csv"), TestMethod]
        public void AssignValuesToTheTestReportTemplate()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            string scenarioTemplate = TestEnvironment.MockPath + TestContext.DataRow["ScenarioTemplate"];
            string featureTemplate = TestEnvironment.MockPath + TestContext.DataRow["FeatureTemplate"];
            string testReportTemplate = TestEnvironment.MockPath + TestContext.DataRow["TestReportTemplate"];

            //When (Act)
            Report r = new Report(p);
            r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);

            //Then (Assert)
            Assert.IsFalse(p.Error);
            Assert.IsNotNull(p.ReportContent, "Report content is not populated for Test Report");
            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(p.Name), "Name on test report template is not populated");
            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(p.Description), "Description on test report template is not populated");
            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(p.Features.Count().ToString()), "Total features on test report template is not populated");
            int totalScenarios = 0;
            int totalScenariosPassed = 0;
            foreach (Feature f in p.Features)
            {
                totalScenarios += f.Scenarios.Count;
                totalScenariosPassed += f.Scenarios.Where(x => x.Passed).ToList<Scenario>().Count;
                if (testReportTemplate.EndsWith("TestReportWithoutFeatureSection.arepa"))
                {
                    Assert.AreEqual(-1, p.ReportContent.IndexOf(f.Title), "Features are populated on test report template without [featuresection]");
                }
                else
                {
                    Assert.AreNotEqual(-1, p.ReportContent.IndexOf(f.Title), "Features are not populated on test report template with [featuresection]");
                }
            }

            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(totalScenarios.ToString()), "Total scenarios on test report template is not populated");
            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(totalScenariosPassed.ToString()), "Total scenarios passed on test report template is not populated");
            Assert.AreNotEqual(-1, p.ReportContent.IndexOf(((totalScenariosPassed * 100) / totalScenarios).ToString()+"%"), "Summary success rate on test report template is not populated");
            //Confirm creation date and time were replaced
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[CreationDate]"));
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[CreationTime]"));
        }

        /// <summary>
        /// @Scenario: Confirm the report file name was generated with the right name
        /// </summary>
        /// <remarks>
        /// Given a Test Project
        /// And one TestReport template
        /// And one Feature template
        /// And one Scenario template
        /// When the Report is generated
        /// Then the name of the report should have the prefix ArepaReport
        /// And the Project title withou spaces
        /// And the Date as suffix
        /// </remarks>
        [TestMethod]
        public void ConfirmTheReportFileNameWasGeneratedWithTheRightName()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            string scenarioTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Scenario.arepa";
            string featureTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\Feature.arepa";
            string testReportTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\TestReport.arepa";

            //When (Act)
            Report r = new Report(p);
            string reportContent = r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);
            string reportFileName = r.SaveReportOnDisc(reportContent, string.Empty);
            //Cleaning test up
            File.Delete(reportFileName);

            //Then (Assert)
            Assert.IsTrue(reportFileName.Contains("ArepaReport_Arepatest_"));
            string date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            Assert.IsTrue(reportFileName.Contains(date));
        }

        /// <summary>
        /// @Scenario: Print out the report file name generated
        /// </summary>
        /// <remarks>
        /// Given a valid test project
        /// When the Application is run
        /// Then the report file name should be printed out on screen
        /// </remarks>
        [TestMethod]
        public void PrintOutTheReportFileNameGenerated()
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
            string errorMessage = "The report file name generated is not displayed on screen";
            string message = proc.StandardOutput.ReadToEnd();
            Regex s = new Regex(resourceMan.Resources.GetString("StringInformationReportGeneratedSuccessfully").Replace("{0}","(.*)"));
            Assert.IsTrue(s.IsMatch(message), errorMessage);
            s = new Regex(resourceMan.Resources.GetString("StringTemplateInformation").Replace("{0}", "(.*)"));
            Assert.IsTrue(s.IsMatch(message), errorMessage);
        }

        /// <summary>
        /// @Scenario: Replace key labels defined mutiple time on template with the right value
        /// </summary>
        /// <remarks>
        /// Given a set of report templates with all values repeated three times
        /// When the template is populated
        /// Then all key values should be replaced
        /// </remarks>
        [TestMethod]
        public void ReplaceKeyLabelsDefinedMutipleTimeOnTemplateWithTheRightValue()
        {
            //Given (Arrange)
            Project p = TestEnvironment.BuildMockProjectWithTest();
            string scenarioTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\ScenarioWithKeyRepeated.arepa";
            string featureTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\FeatureWithKeyRepeated.arepa";
            string testReportTemplate = TestEnvironment.MockPath + @"HtmlTestReport\ReportTemplates\TestReportWithKeyRepeated.arepa";

            //When (Act)
            Report r = new Report(p);
            r.PrepareReportContent(scenarioTemplate, featureTemplate, testReportTemplate);

            //Then (Assert)
            string errorMessage = "Key labels are not replaced properly when it is defined more than once on templates";
            Assert.IsFalse(p.Error);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[ProjectName]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[ProjectDescription]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[SummaryTotalFeatures]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[SummaryTotalScenarios]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[SummaryScenariosPassed]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[SummarySuccessRate]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[FeatureSection]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[CreationDate]"), errorMessage);
            Assert.AreEqual(-1, p.ReportContent.IndexOf("[CreationTime]"), errorMessage);

            foreach (Feature f in p.Features)
            {
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[FeatureTitle]"), errorMessage);
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[FeatureUserStory]"), errorMessage);
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[FeatureTotalScenarios]"), errorMessage);
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[FeatureScenariosPassed]"), errorMessage);
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[FeatureSuccessRate]"), errorMessage);
                Assert.AreEqual(-1, f.ReportContent.IndexOf("[ScenarioSection]"), errorMessage);

                foreach (Scenario s in f.Scenarios)
                {
                    Assert.AreEqual(-1, s.ReportContent.IndexOf("[ScenarioTitle]"), errorMessage);
                    Assert.AreEqual(-1, s.ReportContent.IndexOf("[ScenarioDescription]"), errorMessage);
                    Assert.AreEqual(-1, s.ReportContent.IndexOf("[ScenarioPassed]"), errorMessage);
                    Assert.AreEqual(-1, s.ReportContent.IndexOf("[ScenarioErrorDescription]"), errorMessage);
                }
            }
        }

        /// <summary>
        /// @Scenario: Maintain new lines on html report description
        /// </summary>
        /// <remarks>
        /// Given a user story with multiple lines
        /// And scenario description with multiple lines
        /// When I read the documentation file
        /// Then the user story should have <br/> per new line
        /// And the scenario description should have <br/> per new line
        /// </remarks>
        [TestMethod]
        public void MaintainNewLinesOnHtmlReportDescription()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"DocumentationFiles\ArepaMocks.TestValidDocumentationWith1Scenario1Feature.XML";

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();
            
            projectTested = configParser.ParseDocumentationFile(projectTested, documentationFile);

            //Then (Assert)
            Assert.AreEqual("As lazy user<br/>I want to add two numbers<br/>so that I don't need to use my brain",
                projectTested.Features[0].UserStory,
                "New lines are not replaced by <br/> on user stories");
            Assert.AreEqual("Given a new Add Calculator<br/>When I enter two numbers on screen<br/>Then the result on the screen should be the sum of those two numbers", 
                projectTested.Features[0].Scenarios[0].Description,
                "New lines are not replaced by <br/> on scenario description");
        }

        /// <summary>
        /// @Scenario: Encode the html outputs
        /// </summary>
        /// <remarks>
        /// Given a project name with html tags
        /// And a project description with html tags
        /// And a feature title with html tags
        /// And a user story with html tags
        /// And a scenario title with html tags
        /// And a scenario description with html tags
        /// And an error description with html tags
        /// When the documentation file is read
        /// And the test result file is read
        /// Then all html tags should be encoded
        /// </remarks>
        [TestMethod]
        public void EncodeTheHtmlOutputs()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"HtmlTestReport\ReportProjects\ArepaMocks.TestWithHTMLTags\bin\Debug\ArepaMocks.TestWithHTMLTags.XML";
            string msTestFile = TestEnvironment.MockPath + @"HtmlTestReport\ReportProjects\ArepaMocks.TestWithHTMLTags\TestResults\ArepaMocks.TestWithHTMLTags.trx";

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();
            
            projectTested = configParser.ParseDocumentationFile(projectTested, documentationFile);
            projectTested = configParser.ParseMSTestFile(projectTested, msTestFile, string.Empty);

            //Then (Assert)
            Assert.AreEqual("Mock &lt;h1&gt;Test&lt;/h1&gt;", projectTested.Name, "Project name is not encoded properly");
            Assert.AreEqual("These are &lt;strong&gt;default&lt;/strong&gt; test settings for a local test run.", projectTested.Description, "Project description is not encoded properly");
            Assert.AreEqual("Add Two Numbers", projectTested.Features[0].Title, "Feature title is not encoded properly");
            Assert.AreEqual("As lazy user<br/>I want to add two numbers<br/>so that I don't need to use my brain", projectTested.Features[0].UserStory, "Feature user story is not encoded properly");
            Assert.AreEqual("Add two valid numbers", projectTested.Features[0].Scenarios[0].Title, "Scenario title is not encoded properly");
            Assert.AreEqual("Given a new Add Calculator<br/>When I enter two numbers on screen<br/>Then the result on the screen should be the sum of those two numbers", projectTested.Features[0].Scenarios[0].Description, "Scenario description is not encoded properly");
            Assert.AreEqual("Assert.IsFalse failed. Html error &lt;strong&gt;here&lt;/strong&gt;", projectTested.Features[0].Scenarios[0].ErrorDescription, "Scenario error description is not encoded properly");        
        }
        
    }
}
