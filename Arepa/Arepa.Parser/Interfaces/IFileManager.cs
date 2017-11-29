using System;
namespace Arepa.Parser
{
    /// <summary>
    /// Manages the file directories used by the Application
    /// </summary>
    interface IFileManager
    {
        /// <summary>
        /// Gets the lastest MSTest file name from a given directory
        /// </summary>
        /// <param name="testDirectory">Test directory</param>
        /// <returns>latest MSTest file name</returns>
        string GetMSTestFileName(string testDirectory);

        /// <summary>
        /// Gets the Project File name from a given directory
        /// </summary>
        /// <param name="projectDirectory">Project directory</param>
        /// <returns>Project file name</returns>
        string GetProjectFileName(string projectDirectory);
    }
}
