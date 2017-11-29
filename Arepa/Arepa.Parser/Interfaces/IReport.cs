using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Manages reporting
    /// </summary>
    interface IReport
    {
        /// <summary>
        /// Prepares the html report content based on the reporting templates
        /// </summary>
        /// <param name="scenarioTemplateFile">Scenario Template File</param>
        /// <param name="featureTemplateFile">Feature Template File</param>
        /// <param name="reportTemplateFile">Report Template File</param>
        /// <returns>Report content</returns>
        string PrepareReportContent(string scenarioTemplateFile, string featureTemplateFile, string reportTemplateFile);

        /// <summary>
        /// Saves a string into an html file
        /// </summary>
        /// <param name="htmlContent">content of the file</param>
        /// <returns>Report file name</returns>
        string SaveReportOnDisc(string htmlContent);
    }
}
