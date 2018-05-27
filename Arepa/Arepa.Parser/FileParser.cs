/*  AREPA
 
    A lightweight non-invasive tool that helps you to implement Behaviour Driven Development (BDD) on .NET projects. 
    Arepa produces guidelines of using BDD on your current tests and customisable and portable test reports integrating 
    XML Documentation Comments.

    Copyright (c) 2012 Jose Perez (http://jperez.net/about-me)

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
    to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions 
    of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
    PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
    OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
    DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Resources;
using System.Globalization;

namespace Arepa.Parser
{
    /// <summary>
    /// Convert information from different files into objects on memory
    /// </summary>
    public class FileParser:ParserBase, IFileParser
    {
        #region Fields

        private static string classKey = "T:";
        private static string methodKey = "M:";
        private static string featureKey = "@Feature:";
        private static string scenarioKey = "@Scenario:";
        private static string passedKey = "Passed";
        private static string docDocTag = "doc";
        private static string docMemberTag = "member";
        private static string docNameTag = "name";
        private static string docSummaryTag = "summary";
        private static string docRemarksTag = "remarks";
        private static string docDocumentationFileTag = "DocumentationFile";
        private static string docAssemblyTag = "assembly";
        private static string testTestSettingsTag = "TestSettings";
        private static string testDescriptionTag = "Description";
        private static string testNameTag = "name";
        private static string testTimesTag = "Times";
        private static string testStartTag = "start";
        private static string testFinishTag = "finish";
        private static string testUnitTestTag = "UnitTest";
        private static string testTestMethodTag = "TestMethod";
        private static string testTestCategoryTag = "TestCategory";
        private static string testTestCategoryItemTag = "TestCategoryItem";
        private static string testClassNameTag = "className";
        private static string testIdTag = "id";
        private static string testUnitTestResultTag = "UnitTestResult";
        private static string testTestIdTag = "testId";
        private static string testOutcomeTag = "outcome";
        private static string testMessageTag = "Message";
        private static string testTestResultTag = "TestResult";

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the information content in a XML documentation file into objects on memory
        /// </summary>
        /// <param name="projectTested">Project object</param>
        /// <param name="documentationFile">XML documentation file name</param>
        /// <returns>Project object with data populated from the XML documentation file</returns>
        public Project ParseDocumentationFile(Project projectTested, string documentationFile)
        {
            //Validate projectTested object before use it
            if (projectTested == null)
                return projectTested;

            //Detect if the file exist
            if (!File.Exists(documentationFile))
                return base.ManageError(projectTested, ResourceLabel.StringErrorFileNotFound, new string[] {documentationFile});

            //Read features in file
            XDocument data = XDocument.Load(documentationFile);
            List<XElement> features = (from f in data.Descendants(docDocTag).Descendants(docMemberTag)
                                        where f.Attribute(docNameTag).Value.StartsWith(classKey, StringComparison.OrdinalIgnoreCase)
                                        && f.Element(docSummaryTag).Value.IndexOf(featureKey,StringComparison.OrdinalIgnoreCase)!=-1
                                        select f).ToList<XElement>();

            //Get name from file
            projectTested.Name = data.Descendants(docDocTag).Descendants(docAssemblyTag).Descendants(docNameTag).FirstOrDefault().Value;

            //Detect if feature exist
            if (features.Count == 0)
                return base.ManageError(projectTested, ResourceLabel.StringErrorNoFeaturesFound, new string[] { documentationFile });

            //Variable to detect if there are scenarios without test asscoiated
            int totalScenariosWithTest = 0;

            //Inspect features
            foreach (XElement feature in features)
            {
                //Extract the values from the feature
                string className = ExtractClassNameFromDocumentElement(feature);
                string featureTitle = ExtractFeatureTitleFromDocumentElement(feature);
                string featureUserStory = ExtractUserStoryFromDocumentElement(feature);

                //Detect if title exist         
                if (string.IsNullOrEmpty(featureTitle))
                    return base.ManageError(projectTested, ResourceLabel.StringErrorNoTitleFoundOnFeature, new string[] { className });

                //Detect if user story exist                  
                if (string.IsNullOrEmpty(featureUserStory))
                    return base.ManageError(projectTested, ResourceLabel.StringErrorNoUserStoryOnFeature, new string[] { className });

                //Assign the features values to objects in memory
                Feature f = new Feature();
                f.Title = featureTitle;
                f.UserStory = featureUserStory;
                f.MemberName = className;

                //Get the scenarios associated to this feature
                List<XElement> scenarios = (from s in data.Descendants(docDocTag).Descendants(docMemberTag)
                                            where s.Attribute(docNameTag).Value.StartsWith(methodKey + className, StringComparison.OrdinalIgnoreCase)
                                            && s.Element(docSummaryTag).Value.IndexOf(scenarioKey, StringComparison.OrdinalIgnoreCase) != -1
                                            select s).ToList<XElement>();

                //Detect if scenario exist
                if (scenarios.Count == 0)
                    return base.ManageError(projectTested, ResourceLabel.StringErrorNoScenariosOnFeature, new string[] { featureTitle, className });

                //Inspect Scenarios associated with the feature
                foreach (XElement scenario in scenarios)
                {
                    //Extract the values from the scenario
                    string methodName = ExtractMethodNameFromDocumentElement(scenario);
                    string scenarioTitle = ExtractScenarioTitleFromDocumentElement(scenario);
                    string scenarioDescription = ExtractScenarioDescriptionFromDocumentElement(scenario);

                    //Detect if title exist
                    if (string.IsNullOrEmpty(scenarioTitle))
                        return base.ManageError(projectTested, ResourceLabel.StringErrorNoTitleOnScenario, new string[] { methodName });

                    //Detect if scenario description exist
                    if (string.IsNullOrEmpty(scenarioDescription))
                        return base.ManageError(projectTested, ResourceLabel.StringErrorNoScenarioDescriptionOnMethod, new string[] { scenarioTitle, methodName });

                    //Assign the scenario values to objects in memory
                    Scenario s = new Scenario();
                    s.Title = scenarioTitle;
                    s.Description = scenarioDescription;
                    s.MemberName = methodName;
                    //Add scenario into the collection in the feature
                    f.Scenarios.Add(s);
                    totalScenariosWithTest++;
                }

                //Add feature into the collection in the Project
                projectTested.Features.Add(f);
            } 

            //Detected if there are scenarios without features associated
            List<XElement> allScenarios = (from s in data.Descendants(docDocTag).Descendants(docMemberTag)
                                           where s.Element(docSummaryTag).Value.IndexOf(scenarioKey, StringComparison.OrdinalIgnoreCase) != -1
                                           select s).ToList<XElement>();

            if (allScenarios.Count > totalScenariosWithTest)
            {
                //Detect which scenarios don't have feature associated
                foreach (XElement rawScenario in allScenarios)
                {
                    string memberName = ExtractMethodNameFromDocumentElement(rawScenario);
                    string scenarioTitle = ExtractScenarioTitleFromDocumentElement(rawScenario);
                    //Detect if this raw scenario exist in the scenario with test list
                    foreach (Feature f in projectTested.Features)
                    {
                        Scenario s = f.Scenarios.Where(x => x.MemberName == memberName).FirstOrDefault<Scenario>();
                        //Throw an error if it doesn't exist
                        if (s == null)
                            return base.ManageError(projectTested, ResourceLabel.StringErrorNoFeaturesFoundForScenario, new string[] { scenarioTitle });
                    }
                }
            }

            return projectTested;
        }

        /// <summary>
        /// Parses the information content in a project file into objects on memory
        /// </summary>
        /// <param name="ProjectTested">Project object</param>
        /// <param name="projectFile">Project file name</param>
        /// <returns>Project object with data populated from the project file</returns>
        public Project ParseProjectFile(Project projectTested, string projectFile)
        {
            //Validate projectTested object before use it
            if (projectTested == null)
                return projectTested;

            //Detect if the file exist
            if (!File.Exists(projectFile))
                return base.ManageError(projectTested, ResourceLabel.StringErrorFileNotFound, new string[] { projectFile });

            //Read document file name in file
            XDocument data = XDocument.Load(projectFile);
            XElement documentElement = data.Descendants().Where(x => x.Name.LocalName == docDocumentationFileTag).FirstOrDefault<XElement>();

            //Detect if a documentation file name exist
            if (documentElement == null || string.IsNullOrEmpty(documentElement.Value.Trim()))
                return base.ManageError(projectTested, ResourceLabel.StringErrorDocumentationFileNotFoundInProjectFile, new string[] { projectFile });

            //Assign the documentation file name to the project
            projectTested.DocumentationFile = Directory.GetParent(projectFile).FullName + "\\" + documentElement.Value.Trim();

            return projectTested;
        }

        /// <summary>
        /// Parses the infomration content in a MSTest report file based on scanrios defined on a project
        /// </summary>
        /// <param name="projectTested">Project with scenarios pending for test information</param>
        /// <param name="msTestFile">MSTest file name</param>
        /// <returns>Project with the information related with the test scenarios</returns>
        public Project ParseMSTestFile(Project projectTested, string msTestFile, string testCategory)
        {
            //Set Description to test category if not blank
            projectTested.Description = testCategory != null ? "Filtered Category: " + testCategory : string.Empty;

            //Validates object befor use it
            if (projectTested == null)
                return projectTested;
            
            //Detect if the file exist
            if (!File.Exists(msTestFile))
                return base.ManageError(projectTested, ResourceLabel.StringErrorFileNotFound, new string[] { msTestFile });

            //Read document file name in file
            XDocument data = XDocument.Load(msTestFile);

            //Gets the project name and description supplied in .testsettings. Replaced with test category and assembly name
            /*
            XElement testSettings = data.Descendants().Where(x => x.Name.LocalName == testTestSettingsTag).FirstOrDefault<XElement>();
            XElement description = testSettings.Descendants().Where(x => x.Name.LocalName == testDescriptionTag).FirstOrDefault<XElement>();

            projectTested.Name = testSettings.Attribute(testNameTag).Value.HtmlEncode();
            projectTested.Description = GetProjectDescription(description);
            */
             
            //Read the start and finish time
            XElement times = data.Descendants().Where(x => x.Name.LocalName == testTimesTag).FirstOrDefault<XElement>();
            projectTested.StartTime = Convert.ToDateTime(times.Attribute(testStartTag).Value, CultureInfo.CurrentCulture);
            projectTested.FinishTime = Convert.ToDateTime(times.Attribute(testFinishTag).Value, CultureInfo.CurrentCulture);

            List<XElement> unitTest = data.Descendants().Where(x => x.Name.LocalName == testUnitTestTag).ToList<XElement>();
            bool projectHasTestAsscoiated = false;

            foreach (XElement ut in unitTest)
            {
                List<string> categories = new List<string>();
                string category = string.Empty;
                if (!string.IsNullOrWhiteSpace(testCategory))
                {
                    var cat = ut
                        .Elements()
                        .FirstOrDefault(e => e.Name.LocalName == testTestCategoryTag);
                    if (cat != null)
                    {
                        var catItem = cat
                            .Elements()
                            .Where(e => e.Name.LocalName == testTestCategoryItemTag);

                        categories.AddRange(catItem.Select(y => y.Attribute(testTestCategoryTag).Value));              
                    }
                }

                if (string.IsNullOrWhiteSpace(testCategory) || categories.Contains(testCategory))
                {
                    //Gets the Method name from the Test method
                    XElement tesMethod = ut.Descendants().Where(x => x.Name.LocalName == testTestMethodTag).FirstOrDefault<XElement>();
                    string className = ExtractClassNameFromTestElement(tesMethod.Attribute(testClassNameTag).Value);

                    //Confirms if that className belong to any feature to void continue
                    Feature feature = projectTested.Features.Where(x => x.MemberName == className).FirstOrDefault<Feature>();
                    if (feature != null)
                    {
                        //Gets teh method name of the UnitTest
                        string methodName = className + "." + ut.Attribute(testNameTag).Value;

                        //Gets the scenario related with that unit test
                        Scenario s = feature.Scenarios.Where(x => x.MemberName == methodName).FirstOrDefault<Scenario>();

                        if (s != null)
                        {
                            //Gets the id of the unit test and extract its value
                            string utId = ut.Attribute(testIdTag).Value;
                            XElement scenarioTest = data.Descendants().Where(x => x.Name.LocalName == testUnitTestResultTag).
                                Where(x => x.Attribute(testTestIdTag).Value == utId).FirstOrDefault<XElement>();

                            //Populate scenario values
                            PopulateScenarioValuesFromTestFile(s, scenarioTest, data, utId);

                            //Indicate that the scenario has test asssociated
                            s.HasTest = projectHasTestAsscoiated = true;
                        }
                    }
                }
            }

            //Raise an error if not test was found for at least one scenario
            if(!projectHasTestAsscoiated)
                return base.ManageError(projectTested, ResourceLabel.StringErrorNoTestResultsFound, new string[] { msTestFile, projectTested.DocumentationFile });

            return projectTested;
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Popiulate scenario values
        /// </summary>
        /// <param name="s">Scenario</param>
        /// <param name="scenarioTest">Scenario tests</param>
        /// <param name="data">test data</param>
        /// <param name="unitTestId">unique unit test id</param>
        private static void PopulateScenarioValuesFromTestFile(Scenario s, XElement scenarioTest, XDocument data, string unitTestId)
        {
            if (scenarioTest != null)
            {
                s.Passed = string.Equals(scenarioTest.Attribute(testOutcomeTag).Value, passedKey);
                //Get the error message is scenario didn't pass
                if (!s.Passed)
                    s.ErrorDescription = scenarioTest.Descendants().Where(x => x.Name.LocalName == testMessageTag).FirstOrDefault<XElement>().Value.HtmlEncode();
            }
            else
            {
                //Sets the outcome as error message if no UnitTestResult is found for this test
                XElement testResult = data.Descendants().Where(x => x.Name.LocalName == testTestResultTag).
                Where(x => x.Attribute(testTestIdTag).Value == unitTestId).FirstOrDefault<XElement>();
                s.ErrorDescription = testResult.Attribute(testOutcomeTag).Value.HtmlEncode();
            }
        }

        /// <summary>
        /// Get the project description from MSTest file
        /// </summary>
        /// <param name="description">XElement where the description should be</param>
        /// <returns>Plain project description</returns>
        private static string GetProjectDescription(XElement description)
        {
            if (description == null)
            {
                ResourceFileManager resMan = new ResourceFileManager();
                return resMan.Resources.GetString(ResourceLabel.StringReportDefaultDescription.ToString());
            }
            else
                return description.Value.HtmlEncode();
        }

        /// <summary>
        /// Extract the Class Name from a XElement
        /// </summary>
        /// <param name="feature">XElement with the class name</param>
        /// <returns>Class Name</returns>
        private static string ExtractClassNameFromDocumentElement(XElement feature)
        {
            return feature.Attribute(docNameTag).Value.Remove(0, classKey.Length);
        }

        /// <summary>
        /// Extract the Method Name from a XElement
        /// </summary>
        /// <param name="feature">XElement with the method name</param>
        /// <returns>Method Name</returns>
        private static string ExtractMethodNameFromDocumentElement(XElement method)
        {
            return method.Attribute(docNameTag).Value.Remove(0, methodKey.Length);
        }

        /// <summary>
        /// Extract the summary from an XElment with the "@Feature:" keyword
        /// </summary>
        /// <param name="feature">XElement with the "@Feature:" keyword</param>
        /// <returns>Title of the feature</returns>
        private static string ExtractFeatureTitleFromDocumentElement(XElement feature)
        {
            string title = string.Empty;
            //Extract title only of summary contain the "@Feature:" keyword
            if(feature.Element(docSummaryTag).Value.IndexOf(featureKey,StringComparison.OrdinalIgnoreCase)!=-1)
            {
                title = feature.Element(docSummaryTag).Value.Trim();

                //Removes everything before @Feature:
                title = title.Remove(0, title.IndexOf(featureKey, StringComparison.OrdinalIgnoreCase) + featureKey.Length).Trim();
            }
            return title;
        }

        /// <summary>
        /// Extract the remarks content from an XElement with the "@Feature:" keywork
        /// </summary>
        /// <param name="feature">XElement with the "@Feature:" keywork and a remarks tag</param>
        /// <returns>User story</returns>
        private static string ExtractUserStoryFromDocumentElement(XElement feature)
        {
            string userStory = string.Empty;

            if (feature.Element(docRemarksTag) != null)
                userStory = feature.Element(docRemarksTag).Value.Trim().ReplaceNewLineByBRTag();

            return userStory;
        }

        /// <summary>
        /// Extract the summary from an XElment with the "@Scenario:" keyword
        /// </summary>
        /// <param name="scenario">XElement with the "@Scenario:" keyword</param>
        /// <returns>Title of the scenario</returns>
        private static string ExtractScenarioTitleFromDocumentElement(XElement scenario)
        {
            string title = string.Empty;
            //Extract title only of summary contain the "@Feature:" keyword
            if (scenario.Element(docSummaryTag).Value.IndexOf(scenarioKey, StringComparison.OrdinalIgnoreCase) != -1)
            {
                title = scenario.Element(docSummaryTag).Value.Trim();

                //Removes everything before @Feature:
                title = title.Remove(0, title.IndexOf(scenarioKey, StringComparison.OrdinalIgnoreCase) + scenarioKey.Length).Trim();
            }
            return title;
        }

        /// <summary>
        /// Extract the remarks content from an XElement with the "@Scenario:" keywork
        /// </summary>
        /// <param name="feature">XElement with the "@Scenario:" keywork and a remarks tag</param>
        /// <returns>Scenario description</returns>
        private static string ExtractScenarioDescriptionFromDocumentElement(XElement scenario)
        {
            string scenarioDescription = string.Empty;

            if (scenario.Element(docRemarksTag) != null)
                scenarioDescription = scenario.Element(docRemarksTag).Value.Trim().ReplaceNewLineByBRTag();

            return scenarioDescription;
        }

        /// <summary>
        /// Extract the class name from the class name value string
        /// </summary>
        /// <param name="classNameValue">class name value returned from the className attribute</param>
        /// <returns>Class name</returns>
        private static string ExtractClassNameFromTestElement(string classNameValue)
        {
            return classNameValue.Substring(0,classNameValue.IndexOf(','));
        }

        #endregion

    }
}
