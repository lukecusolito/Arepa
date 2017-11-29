using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Represents an scenario on memory
    /// </summary>
    interface IScenario
    {
        /// <summary>
        /// Gherkin sentence
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Test Error description
        /// </summary>
        string ErrorDescription { get; set; }

        /// <summary>
        /// Indicates if the scenario has a test asssociated
        /// </summary>
        bool HasTest { get; set; }

        /// <summary>
        /// Name of the method
        /// </summary>
        string MemberName { get; set; }

        /// <summary>
        /// Indicates if the scenario has a test passed
        /// </summary>
        bool Passed { get; set; }

        /// <summary>
        /// Content to be displayed on the test report
        /// </summary>
        string ReportContent { get; set; }

        /// <summary>
        /// Title of the Scenario
        /// </summary>
        string Title { get; set; }
    }
}
