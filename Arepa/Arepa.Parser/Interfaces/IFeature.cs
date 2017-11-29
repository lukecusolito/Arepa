using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Represents a feature on memory
    /// </summary>
    interface IFeature
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        string MemberName { get; set; }

        /// <summary>
        /// Content to be displayed on the test report
        /// </summary>
        string ReportContent { get; set; }

        /// <summary>
        /// Scenarios assciated with this feature
        /// </summary>
        System.Collections.ObjectModel.Collection<Arepa.Parser.Scenario> Scenarios { get; set; }

        /// <summary>
        /// Feature Title defined on the XML documentation file
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// User story defined on the XML documentation file
        /// </summary>
        string UserStory { get; set; }
    }
}
