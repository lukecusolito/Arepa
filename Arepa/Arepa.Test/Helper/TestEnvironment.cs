using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using Arepa.Parser;

namespace Arepa.Test.Helper
{
    /// <summary>
    /// Testing Environment Properties and Methods
    /// </summary>
    public static class TestEnvironment
    {
        /// <summary>
        /// Defines the full path where the mocks should be located
        /// </summary>
        public static string MockPath
        {
            get { return Path.Combine(Environment.CurrentDirectory, @"..\..\..\Arepa.Test\Mocks\"); }
        }

        /// <summary>
        /// Defines the full path where the console should be located (Debug)
        /// </summary>
        public static string ConsolePath
        {
            get { return Path.Combine(Environment.CurrentDirectory, @"..\..\..\Arepa\Bin\Debug\"); }
        }

        /// <summary>
        /// Prepares a message string to be analised as regular expression
        /// </summary>
        /// <param name="errorMessage">Raw message</param>
        /// <param name="strict">Define if regular expression must start and end with the errorMessage</param>
        /// <returns>Message formated with regular expression</returns>
        public static string PrepareStringForRegularExpression(string errorMessage, bool strict)
        {
            //Replace regex reserved characters
            errorMessage = errorMessage.Replace(".", "(.*)");
            errorMessage = errorMessage.Replace("[", "(.*)");
            errorMessage = errorMessage.Replace("\\", "(.*)");
            errorMessage = errorMessage.Replace("^", "(.*)");
            errorMessage = errorMessage.Replace("$", "(.*)");
            errorMessage = errorMessage.Replace("|", "(.*)");
            errorMessage = errorMessage.Replace("?", "(.*)");
            errorMessage = errorMessage.Replace("+", "(.*)");

            //Replace parameters
            errorMessage = errorMessage.Replace("{0}", "(.*)");
            errorMessage = errorMessage.Replace("{1}", "(.*)");
            errorMessage = errorMessage.Replace("{2}", "(.*)");
            //add start and end values
            if(strict)
                errorMessage = "^" + errorMessage + "$";
            return errorMessage;
        }

        /// <summary>
        /// Builds a project without test to be used on tests
        /// </summary>
        /// <returns>Mock Project without test</returns>
        public static Project BuildMockProjectWithoutTest()
        {
            Project p = new Project();

            Feature f1 = new Feature();
            f1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2";
            Scenario s1 = new Scenario();
            s1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario3";
            Scenario s2 = new Scenario();
            s2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario4";
            f1.Scenarios = new Collection<Scenario> { s1, s2 };

            Feature f2 = new Feature();
            f2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1";
            Scenario s3 = new Scenario();
            s3.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario1";
            Scenario s4 = new Scenario();
            s4.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario2";
            f2.Scenarios = new Collection<Scenario> { s3, s4 };

            Feature f3 = new Feature();
            f3.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1";
            Scenario s5 = new Scenario();
            s5.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1.BasicUnitTest1_TestMethod1";
            f3.Scenarios = new Collection<Scenario> { s5 };

            Feature f4 = new Feature();
            f4.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1";
            Scenario s6 = new Scenario();
            s6.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1.CodedUITestMethod1";
            f4.Scenarios = new Collection<Scenario> { s6 };

            p.Features = new Collection<Feature> { f1, f2, f3, f4 };

            return p;
        }

        /// <summary>
        /// Builds a project with test to be used on tests
        /// </summary>
        /// <returns>Mock Project with test</returns>
        public static Project BuildMockProjectWithTest()
        {
            Project p = new Project();
            p.Name = "Arepa test";
            p.Description = "Project to test Arepa";
            p.StartTime = DateTime.Now.AddHours(-1);
            p.FinishTime = DateTime.Now.AddMinutes(18);

            Feature f1 = new Feature();
            f1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2";
            f1.Title = "Feature Title 1";
            f1.UserStory = "As a tester I want to have Feature 1 so that I can test Arepa";
   
            Scenario s1 = new Scenario();
            s1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario3";
            s1.Title = "Scenario Title 1";
            s1.Description = "Given a Scenario 1 When I tested it Then something should happens";
            s1.HasTest = true;
            s1.Passed = true;

            Scenario s2 = new Scenario();
            s2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario4";
            s2.Title = "Scenario Title 2";
            s2.Description = "Given a Scenario 2 When I tested it Then something should happens";
            s2.HasTest = true;
            s2.Passed = false;
            s2.ErrorDescription = "Expected 10 and the result was 29";
            f1.Scenarios = new Collection<Scenario> { s1, s2 };

            Feature f2 = new Feature();
            f2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1";
            f2.Title = "Feature Title 2";
            f2.UserStory = "As a tester I want to have Feature 2 so that I can test Arepa";

            Scenario s3 = new Scenario();
            s3.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario1";
            s3.Title = "Scenario Title 3";
            s3.Description = "Given a Scenario 3 When I tested it Then something should happens";
            s3.HasTest = true;
            s3.Passed = true;

            Scenario s4 = new Scenario();
            s4.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario2";
            s4.Title = "Scenario Title 4";
            s4.Description = "Given a Scenario 4 When I tested it Then something should happens";
            s4.HasTest = true;
            s4.Passed = true;
            f2.Scenarios = new Collection<Scenario> { s3, s4 };

            Feature f3 = new Feature();
            f3.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1";
            f3.Title = "Feature Title 3";
            f3.UserStory = "As a tester I want to have Feature 3 so that I can test Arepa";

            Scenario s5 = new Scenario();
            s5.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1.BasicUnitTest1_TestMethod1";
            s5.Title = "Scenario Title 5";
            s5.Description = "Given a Scenario 5 When I tested it Then something should happens";
            s5.HasTest = true;
            s5.Passed = true;
            f3.Scenarios = new Collection<Scenario> { s5 };

            Feature f4 = new Feature();
            f4.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1";
            f4.Title = "Feature Title 4";
            f4.UserStory = "As a tester I want to have Feature 4 so that I can test Arepa";

            Scenario s6 = new Scenario();
            s6.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1.CodedUITestMethod1";
            s6.Title = "Scenario Title 6";
            s6.Description = "Given a UI Test Scenario 6 When I tested it Then something should happens";
            s6.HasTest = true;
            s6.Passed = true;
            f4.Scenarios = new Collection<Scenario> { s6 };

            p.Features = new Collection<Feature> { f1, f2, f3, f4 };

            return p;
        }

        /// <summary>
        /// Builds a project with both scenarios with tests and scenarios without tests
        /// </summary>
        /// <returns>Mock Project with both scenarios with tests and scenarios without tests</returns>
        public static Project BuildMockProjectWithScenariosWithBothTestAndNoTest()
        {
            Project p = new Project();
            p.Name = "Arepa test";
            p.Description = "Project to test Arepa";
            p.StartTime = DateTime.Now.AddHours(-1);
            p.FinishTime = DateTime.Now.AddMinutes(18);

            Feature f1 = new Feature();
            f1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2";
            f1.Title = "Feature Title 1";
            f1.UserStory = "As a tester I want to have Feature 1 so that I can test Arepa";

            Scenario s1 = new Scenario();
            s1.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario3";
            s1.Title = "Scenario Title 1";
            s1.Description = "Given a Scenario 1 When I tested it Then something should happens";
            s1.HasTest = true;
            s1.Passed = true;

            Scenario s2 = new Scenario();
            s2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess2.Scenario4";
            s2.Title = "Scenario Title 2";
            s2.Description = "Given a Scenario 2 When I tested it Then something should happens";
            s2.HasTest = true;
            s2.Passed = false;
            s2.ErrorDescription = "Expected 10 and the result was 29";
            f1.Scenarios = new Collection<Scenario> { s1, s2 };

            Feature f2 = new Feature();
            f2.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1";
            f2.Title = "Feature Title 2";
            f2.UserStory = "As a tester I want to have Feature 2 so that I can test Arepa";

            Scenario s3 = new Scenario();
            s3.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario1";
            s3.Title = "Scenario Title 3";
            s3.Description = "Given a Scenario 3 When I tested it Then something should happens";
            s3.HasTest = true;
            s3.Passed = true;

            Scenario s4 = new Scenario();
            s4.MemberName = "ArepaMocks.TestValidTestReport.FeatureSuccess1.Scenario2";
            s4.Title = "Scenario Title 4";
            s4.Description = "Given a Scenario 4 When I tested it Then something should happens";
            s4.HasTest = true;
            s4.Passed = true;
            f2.Scenarios = new Collection<Scenario> { s3, s4 };

            Feature f3 = new Feature();
            f3.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1";
            f3.Title = "Feature Title 3";
            f3.UserStory = "As a tester I want to have Feature 3 so that I can test Arepa";

            Scenario s5 = new Scenario();
            s5.MemberName = "ArepaMocks.TestValidTestReport.BasicUnitTest1.BasicUnitTest1_TestMethod1";
            s5.Title = "Scenario Title 5";
            s5.Description = "Given a Scenario 5 When I tested it Then something should happens";
            s5.HasTest = true;
            s5.Passed = true;
            f3.Scenarios = new Collection<Scenario> { s5 };

            Feature f4 = new Feature();
            f4.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1";
            f4.Title = "Feature Title 4";
            f4.UserStory = "As a tester I want to have Feature 4 so that I can test Arepa";

            Scenario s6 = new Scenario();
            s6.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1.CodedUITestMethod1";
            s6.Title = "Scenario Title 6";
            s6.Description = "Given a UI Test Scenario 6 When I tested it Then something should happens";
            s6.HasTest = true;
            s6.Passed = true;

            Scenario s7 = new Scenario();
            s7.MemberName = "ArepaMocks.TestValidTestReport.CodedUITest1.ScenarioWithoutTest";
            s7.Title = "Scenario Title 7";
            s7.Description = "Given a UI Test Scenario 7 When I tested it Then something should happens";
            s7.HasTest = false;

            f4.Scenarios = new Collection<Scenario> { s6, s7 };

            p.Features = new Collection<Feature> { f1, f2, f3, f4 };

            return p;
        }
    }
}
