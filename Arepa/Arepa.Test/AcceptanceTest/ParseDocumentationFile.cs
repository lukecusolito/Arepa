using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arepa.Parser;
using Arepa.Test.Helper;
using System.Text.RegularExpressions;

namespace Arepa.Test.AcceptanceTest
{
    /// <summary>
    /// @Feature: Parse Documentation File
    /// </summary>
    /// <remarks>
    /// As a tester 
    /// I want to parse the test xml documentation file 
    /// so that I can import the feature and scenario information
    /// </remarks>
    [TestClass]
    public class ParseDocumentationFile
    {

        private ResourceFileManager resourceMan = null;
        private TestContext testContextInstance;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParseDocumentationFile()
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

        #region Acceptance Tests

        /// <summary>
        /// @Scenario: Detect if the documentation file exits
        /// </summary>
        /// <remarks>
        /// Given a file name
        /// When the application doesn't found the file
        /// Then an error should be returned
        /// </remarks>
        [TestMethod]
        public void DetectIfTheDocumentationFileExits()
        {

            //Given (Arrange)
            string documentationFile = @"non-existentFile.xml";

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString("StringErrorFileNotFound"));

        }

        /// <summary>
        /// @Scenario: Detect if at least one feature exist in a documentation file
        /// </summary>
        /// <remarks>
        /// Given a documentation file
        /// When a feature is not found
        /// Then an error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfFeatureExistDocumentFile()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"DocumentationFiles\ArepaMocks.TestWithoutFeatures.XML";

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString("StringErrorNoFeaturesFound"));
        }

        /// <summary>
        /// @Scenario: Detect if at least on scenario per feature exist in a documentation file
        /// </summary>
        /// <remarks>
        /// Given a documentation file
        /// And a feature
        /// When a scenario doesn't exist in that feature
        /// Then an error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfOneScenarioPerFeatureExistDocumentFile()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"DocumentationFiles\ArepaMocks.TestWithoutScenarios.XML";

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString("StringErrorNoScenariosOnFeature"));
        }

        /// <summary>
        /// @Scenario: Detect if title and user story exist in a documentation file
        /// </summary>
        /// <remarks>
        /// Given a documentation file
        /// And a feature
        /// When the feature title doesn't exist
        /// Or the user story doesn't exist
        /// Then an error should be raised
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Sources\\DocumentationFiles\\DocumentationFilesBadFeature.csv", "DocumentationFilesBadFeature#csv", DataAccessMethod.Sequential), DeploymentItem("Arepa.Test\\Sources\\DocumentationFilesBadFeature.csv"), TestMethod]
        public void DetectIfTitleAndUserStoryExistInFeaturesDocumentFile()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + TestContext.DataRow["DocumentationFileName"];

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString(TestContext.DataRow["ErrorMsg"].ToString()));
        }

        /// <summary>
        /// @Scenario: Detect if title and scenario content exist in scenarios in a document file
        /// </summary>
        /// <remarks>
        /// Given a documentation file
        /// And a Scenario
        /// When the scenario title doesn't exist
        /// Or the scenario content doesn't exist
        /// Then an error should be raised
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Sources\\DocumentationFiles\\DocumentationFilesBadScenario.csv", "DocumentationFilesBadScenario#csv", DataAccessMethod.Sequential), DeploymentItem("Arepa.Test\\Sources\\DocumentationFilesBadScenario.csv"), TestMethod]
        public void DetectIfTitleAndScenarioContentExistInScenariosDocumentFile()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + TestContext.DataRow["DocumentationFileName"];

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString(TestContext.DataRow["ErrorMsg"].ToString()));
        }

        /// <summary>
        /// @Scenario: Convert data in Documentation file into objects
        /// </summary>
        /// <remarks>
        /// Given a valid documentation file
        /// When there are features (case-insensitive)
        /// And Scenarios (case-insensitive) on those features
        /// Then Features should be on objects in memory
        /// And Scenarios should be on objects in memory
        /// </remarks>
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\Sources\\DocumentationFiles\\ValidDocumentationFiles.csv", "ValidDocumentationFiles#csv", DataAccessMethod.Sequential), DeploymentItem("Arepa.Test\\Sources\\ValidDocumentationFiles.csv"), TestMethod]
        public void ConvertDataInDocumentationFileIntoObjects()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + TestContext.DataRow["DocumentationFileName"];

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();
            projectTested = configParser.ParseDocumentationFile(projectTested, documentationFile);

            //Then (Assert)
            int totalScenarios = 0;
            Assert.IsFalse(projectTested.Error);
            Assert.IsNotNull(projectTested.Features);
            Assert.AreEqual(Convert.ToInt32(TestContext.DataRow["TotalFeatures"]), projectTested.Features.Count);
            foreach (Feature feature in projectTested.Features)
            {
                Assert.IsNotNull(feature.Scenarios);
                //Ensures at least one scenario per feature
                Assert.AreNotEqual(0, feature.Scenarios.Count);
                totalScenarios += feature.Scenarios.Count;
            }
            Assert.AreEqual(Convert.ToInt32(TestContext.DataRow["TotalScenarios"]), totalScenarios);
        }

        /// <summary>
        /// @Scenario: Get the feature member name
        /// </summary>
        /// <remarks>
        /// Given a Documentation file
        /// When the features are read
        /// Then the member name should be assigned to the feature
        /// </remarks>
        [TestMethod]
        public void GetTheFeatureMemberName()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"DocumentationFiles\ArepaMocks.TestValidDocumentationWith1Scenario1Feature.XML";

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();
            projectTested = configParser.ParseDocumentationFile(projectTested, documentationFile);

            //Then (Assert)
            Assert.IsFalse(projectTested.Error);
            Assert.AreEqual("ArepaMocks.TestValidDocumentationWith1Scenario1Feature.AddTwoNumbers", projectTested.Features[0].MemberName);
        }

        /// <summary>
        /// @Scenario: Detect if all scenarios have feature associated
        /// </summary>
        /// <remarks>
        /// Given a documentation file with scenarios without features
        /// When a the documentation file is parsed
        /// Then an error should be raised
        /// </remarks>
        [TestMethod]
        public void DetectIfAllScenariosHaveFeatureAssociated()
        {
            //Given (Arrange)
            string documentationFile = TestEnvironment.MockPath + @"DocumentationFiles\ArepaMocks.TestScenariosWithoutFeatures.XML";

            //When (Act) and Then (Assert)
            ParseDocumentationFileWhenThen(documentationFile, resourceMan.Resources.GetString("StringErrorNoFeaturesFoundForScenario"));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs the common when and then of the test scanrios for this feature
        /// </summary>
        /// <param name="documentationFile">Mock documentation file to test</param>
        /// <param name="errorMsgExpected">Error message expected</param>
        private static void ParseDocumentationFileWhenThen(string documentationFile, string errorMsgExpected)
        {

            //When (Act)
            FileParser configParser = new FileParser();
            Project projectTested = new Project();

            Message msg = null;
            configParser.MessageRaised += delegate(object sender, MessageEventArgs e)
            {
                msg = e.MsgRaised;
            };

            projectTested = configParser.ParseDocumentationFile(projectTested, documentationFile);


            //Then (Assert)
            Assert.IsTrue(projectTested.Error);
            Assert.AreEqual(MessageType.Error, msg.TypeMessage);
            //Ensures it is picking the right error message
            Regex s = new Regex(TestEnvironment.PrepareStringForRegularExpression(errorMsgExpected,true));
            Assert.IsTrue(s.IsMatch(msg.Description));
        }

        

        #endregion
    }
}
