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
using System.Text.RegularExpressions;
using System.Globalization;

namespace Arepa.Parser
{
    /// <summary>
    /// Manages the file directories used by the Application
    /// </summary>
    public class FileManager:ParserBase, IFileManager
    {

        #region Public Methods

        /// <summary>
        /// Gets the Project File name from a given directory
        /// </summary>
        /// <param name="projectDirectory">Project directory</param>
        /// <returns>Project file name</returns>
        public string GetProjectFileName(string projectDirectory)
        {
            string projectFileName = GetFileName(projectDirectory, FileType.ProjectFile);

            if (string.IsNullOrEmpty(projectFileName))
                base.OnMessageRaised(new Message(MessageType.Error, ResourceLabel.StringErrorNoProjectFileFound, new string[] { projectDirectory }));

            return projectFileName;
        }

        /// <summary>
        /// Gets the lastest MSTest file name from a given directory
        /// </summary>
        /// <param name="testDirectory">Test directory</param>
        /// <returns>latest MSTest file name</returns>
        public string GetMSTestFileName(string testDirectory)
        {
            string msTestFile = GetFileName(testDirectory, FileType.MSTestFile);

            if (string.IsNullOrEmpty(msTestFile))
                base.OnMessageRaised(new Message(MessageType.Error, ResourceLabel.StringErrorNoMSTestFileFound, new string[] { testDirectory }));

            return msTestFile;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the latest file name from a directory and based on the type specified
        /// </summary>
        /// <param name="directory">Directory to analise</param>
        /// <param name="type">Type of file to get</param>
        /// <returns>Latest file found</returns>
        private static string GetFileName(string directory, FileType type)
        {
            string fileName = string.Empty;
            if (Directory.Exists(directory))
            {
                // Process the list of files found in the directory.
                var dir = new DirectoryInfo(directory);
                //Get the files in directory 
                FileInfo fi = null;
                switch (type)
                {
                    case FileType.ProjectFile:
                        fi = (dir.GetFiles("*.csproj").OrderByDescending(f => f.LastWriteTime)).FirstOrDefault<FileInfo>();
                        break;
                    case FileType.MSTestFile:
                        fi = (dir.GetFiles("*.trx").OrderByDescending(f => f.LastWriteTime)).FirstOrDefault<FileInfo>();
                        break;
                }
                if(fi!=null)
                    fileName = fi.FullName;
            }
            return fileName;
        }

        #endregion

        /// <summary>
        /// File Type
        /// </summary>
        private enum FileType
        {
            ProjectFile,
            MSTestFile
        }
    }

    
}
