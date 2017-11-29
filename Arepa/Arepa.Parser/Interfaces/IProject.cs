using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Represent a test project on memory
    /// </summary>
    interface IProject
    {
        /// <summary>
        /// Description defines on test settings
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// XML Documentation file associated to the project
        /// </summary>
        string DocumentationFile { get; set; }

        /// <summary>
        /// Indicates if the project contains errors
        /// </summary>
        bool Error { get; set; }

        /// <summary>
        /// Features associated to te project
        /// </summary>
        System.Collections.ObjectModel.Collection<Arepa.Parser.Feature> Features { get; set; }

        /// <summary>
        /// Test Finish Time
        /// </summary>
        DateTime FinishTime { get; set; }

        /// <summary>
        /// Name defined on test settings
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Content to be displayed on the test report
        /// </summary>
        string ReportContent { get; set; }

        /// <summary>
        /// Test Start Time
        /// </summary>
        DateTime StartTime { get; set; }
    }
}
