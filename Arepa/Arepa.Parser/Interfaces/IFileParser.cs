using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Convert information from different files into objects on memory
    /// </summary>
    interface IFileParser
    {
        /// <summary>
        /// Parses the information content in a XML documentation file into objects on memory
        /// </summary>
        /// <param name="projectTested">Project object</param>
        /// <param name="documentationFile">XML documentation file name</param>
        /// <returns>Project object with data populated from the XML documentation file</returns>
        Project ParseDocumentationFile(Project projectTested, string documentationFile);

        /// <summary>
        /// Parses the infomration content in a MSTest report file based on scanrios defined on a project
        /// </summary>
        /// <param name="projectTested">Project with scenarios pending for test information</param>
        /// <param name="msTestFile">MSTest file name</param>
        /// <returns>Project with the information related with the test scenarios</returns>
        Project ParseMSTestFile(Project projectTested, string msTestFile, string testCategory);

        /// <summary>
        /// Parses the information content in a project file into objects on memory
        /// </summary>
        /// <param name="ProjectTested">Project object</param>
        /// <param name="projectFile">Project file name</param>
        /// <returns>Project object with data populated from the project file</returns>
        Project ParseProjectFile(Project projectTested, string projectFile);
    }
}
