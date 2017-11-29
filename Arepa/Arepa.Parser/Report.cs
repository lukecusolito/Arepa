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
using System.Globalization;

namespace Arepa.Parser
{
    /// <summary>
    /// Manages reporting
    /// </summary>
    public class Report : ParserBase, IReport
    {
        #region Fields

        private const string keyScenarioTitle = "[ScenarioTitle]";
        private const string keyScenarioDescription = "[ScenarioDescription]";
        private const string keyScenarioPassed = "[ScenarioPassed]";
        private const string keyScenarioErrorDescription = "[ScenarioErrorDescription]";
        private const string keyFeatureTitle = "[FeatureTitle]";
        private const string keyFeatureUserStory = "[FeatureUserStory]";
        private const string keyFeatureTotalScenarios = "[FeatureTotalScenarios]";
        private const string keyFeatureScenariosPassed = "[FeatureScenariosPassed]";
        private const string keyFeatureSuccessRate = "[FeatureSuccessRate]";
        private const string keyScenarioSection = "[ScenarioSection]";
        private const string keyProjectName = "[ProjectName]";
        private const string keyProjectDescription = "[ProjectDescription]";
        private const string keySummaryTotalFeatures = "[SummaryTotalFeatures]";
        private const string keySummaryTotalScenarios = "[SummaryTotalScenarios]";
        private const string keySummaryScenariosPassed = "[SummaryScenariosPassed]";
        private const string keySummarySuccessRate = "[SummarySuccessRate]";
        private const string keyFeatureSection = "[FeatureSection]";
        private const string keyCreationDate = "[CreationDate]";
        private const string keyCreationTime = "[CreationTime]";

        // Manages the resource
        ResourceFileManager resMan = null;
        private Project reportProject = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="project">Project to use</param>
        public Report(Project project)
        {
            this.reportProject = project;
            resMan = new ResourceFileManager();
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Prepares the html report content based on the reporting templates
        /// </summary>
        /// <param name="scenarioTemplateFile">Scenario Template File</param>
        /// <param name="featureTemplateFile">Feature Template File</param>
        /// <param name="reportTemplateFile">Report Template File</param>
        /// <returns>Report content</returns>
        public string PrepareReportContent(string scenarioTemplateFile, string featureTemplateFile, string reportTemplateFile)
        {
            //Detect if the files exist
            if (!File.Exists(scenarioTemplateFile) || !File.Exists(featureTemplateFile) || !File.Exists(reportTemplateFile))
            {
                base.ManageError(this.reportProject, ResourceLabel.StringErrorReportTemplateNotFound, null);
                return null;
            }

            //Reads all content from scenario template
            string scenarioTemplateContent = File.ReadAllText(scenarioTemplateFile);
            
            //Populate scenario report content
            PrepareScenarios(scenarioTemplateContent);
            
            //Reads all content from feature template
            string featureTemplateContent = File.ReadAllText(featureTemplateFile);
            
            //Populate feature report content
            PrepareFeatures(featureTemplateContent);
            
            //Reads all content from the test report template
            string testReportTemplateContent = File.ReadAllText(reportTemplateFile);
            
            //Populate the test report content
            PrepareTestReport(testReportTemplateContent);

            //Return the report content for this project
            return reportProject.ReportContent;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populate the report content for all scenarios
        /// </summary>
        /// <param name="scenarioTemplateContent">Scenario template content</param>
        private void PrepareScenarios(string scenarioTemplateContent)
        {            
            //Reads all content from scenario template and replace key values
            foreach (Feature f in reportProject.Features)
            {
                foreach (Scenario s in f.Scenarios)
                {
                    //Only scenarios with test are populated with report content
                    if (s.HasTest)
                    {
                        string sTemplate = scenarioTemplateContent;
                        sTemplate = sTemplate.ReplaceCaseInsensitiveValues(keyScenarioTitle, s.Title);
                        sTemplate = sTemplate.ReplaceCaseInsensitiveValues(keyScenarioDescription, s.Description);
                        sTemplate = sTemplate.ReplaceCaseInsensitiveValues(keyScenarioPassed, PassedStringResult(s.Passed));
                        sTemplate = sTemplate.ReplaceCaseInsensitiveValues(keyScenarioErrorDescription, s.ErrorDescription);

                        //Assign the report content back to the scenario
                        s.ReportContent = sTemplate;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the right value to inform if a scenario was passed or not
        /// </summary>
        /// <param name="passed">passed</param>
        /// <returns>Value localized</returns>
        private string PassedStringResult(bool passed)
        {
            if (passed)
                return resMan.Resources.GetString(ResourceLabel.StringReportYes.ToString());
            else
                return resMan.Resources.GetString(ResourceLabel.StringReportNo.ToString());
        }

        private void PrepareFeatures(string featureTemplateContent)
        {
            //Reads all content from feature template and replace key values
            foreach (Feature f in reportProject.Features)
            {
                string fTemplate = featureTemplateContent;
                int totalScenarios = f.Scenarios.Where(x => x.HasTest == true).ToList<Scenario>().Count;
                int totalScenariosPassed = f.Scenarios.Where(x => x.HasTest == true).Where(x => x.Passed == true).ToList<Scenario>().Count();

                //Ensure the feature has scenarios with test
                if (totalScenarios > 0)
                {
                    fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyFeatureTitle, f.Title);
                    fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyFeatureUserStory, f.UserStory);
                    fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyFeatureTotalScenarios, totalScenarios.ToString(CultureInfo.InvariantCulture));
                    fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyFeatureScenariosPassed, totalScenariosPassed.ToString(CultureInfo.InvariantCulture));
                    fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyFeatureSuccessRate, GetSuccessRate(totalScenarios, totalScenariosPassed));

                    //Replace scenario section if it exist
                    if (fTemplate.IndexOf(keyScenarioSection, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        StringBuilder scenarioContent = new StringBuilder();
                        foreach (Scenario s in f.Scenarios)
                            scenarioContent.Append(s.ReportContent);

                        fTemplate = fTemplate.ReplaceCaseInsensitiveValues(keyScenarioSection, scenarioContent.ToString());
                    }

                    //Assign the report content back to the feature
                    f.ReportContent = fTemplate;
                }
            }
        }

        /// <summary>
        /// Populate de project content report
        /// </summary>
        /// <param name="testReportTemplateContent">TestReport template with the keys to be replaced</param>
        private void PrepareTestReport(string testReportTemplateContent)
        {
            //Reads all content from feature template and replace key values
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyProjectName, reportProject.Name);
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyProjectDescription, reportProject.Description);
            //get total features with scenarios
            int totalFeatures = 0;
            foreach(Feature f in reportProject.Features)
                if (f.Scenarios.Where(x => x.HasTest == true).ToList<Scenario>().Count > 0)
                    totalFeatures++;

            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keySummaryTotalFeatures, totalFeatures.ToString(CultureInfo.InvariantCulture));

            int totalScenarios = 0;
            int totalScenariosPassed = 0;
            bool hasFeatureContent = !int.Equals(testReportTemplateContent.IndexOf(keyFeatureSection, StringComparison.OrdinalIgnoreCase),-1);
            bool hasScenarioContent = !int.Equals(testReportTemplateContent.IndexOf(keyScenarioSection, StringComparison.OrdinalIgnoreCase), -1);
            StringBuilder featureReportContent = new StringBuilder();
            StringBuilder scenarioReportContent = new StringBuilder();

            foreach (Feature f in reportProject.Features)
            {
                //Gets total scenarios and total scenarios passed
                totalScenarios += f.Scenarios.Where(x => x.HasTest == true).ToList<Scenario>().Count;
                totalScenariosPassed += f.Scenarios.Where(x => x.HasTest == true).Where(x => x.Passed == true).ToList<Scenario>().Count;
                
                //Get Feature report content if one featuresection key is found in the report template
                if (hasFeatureContent)
                {
                    featureReportContent.Append(f.ReportContent);
                    //Get Scenario report content if one scenariosection key is found in the report template
                    if (hasScenarioContent)
                        foreach (Scenario s in f.Scenarios)
                            scenarioReportContent.Append(s.ReportContent);
                }
            }

            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keySummaryTotalScenarios, totalScenarios.ToString(CultureInfo.InvariantCulture));
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keySummaryScenariosPassed, totalScenariosPassed.ToString(CultureInfo.InvariantCulture));
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keySummarySuccessRate, GetSuccessRate(totalScenarios,totalScenariosPassed));
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyFeatureSection, featureReportContent.ToString());
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyScenarioSection, scenarioReportContent.ToString());
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyCreationDate, DateTime.Now.ToLongDateString());
            testReportTemplateContent = testReportTemplateContent.ReplaceCaseInsensitiveValues(keyCreationTime, DateTime.Now.ToLongTimeString());

            //Assign the report content back to the project
            reportProject.ReportContent = testReportTemplateContent;
        }

        /// <summary>
        /// Returns the success rate based on two values
        /// </summary>
        /// <param name="totalValue">Total value</param>
        /// <param name="totalSuccess">Total success value</param>
        /// <returns>Success rate in percentaje</returns>
        private static string GetSuccessRate(int totalValue, int totalSuccess)
        {
            return Convert.ToInt32((totalSuccess * 100) / totalValue) + "%";
        }

        /// <summary>
        /// Saves a string into an html file
        /// </summary>
        /// <param name="htmlContent">content of the file</param>
        /// <returns>Report file name</returns>
        public string SaveReportOnDisc(string htmlContent)
        {
            DateTime now = DateTime.Now;
            string reportsDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Reports\";
            string reportFileName = reportsDirectory + "ArepaReport_" + reportProject.Name.Replace(" ", string.Empty) + "_" +
                now.Year + "-" + now.Month + "-" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + ".html";

            //Ensures the directory exist
            if(!Directory.Exists(reportsDirectory))
                Directory.CreateDirectory(reportsDirectory);

            //Save the report
            File.WriteAllText(reportFileName, htmlContent);

            //Returns the name of the report
            return reportFileName;
        }

        #endregion
    }
}
