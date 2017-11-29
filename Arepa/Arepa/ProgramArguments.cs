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
using System.Text.RegularExpressions;
using Arepa.Parser;
using System.Globalization;
using System.IO;

namespace Arepa
{
    /// <summary>
    /// Manages the arguments entered into the console
    /// </summary>
    public class ProgramArguments
    {
        #region Fields

        private const string projectDirArgument = "-PROJECTDIR";
        private const string testDirArgument = "-TESTDIR";
        private const string testCategoryArgument = "-TESTCATEGORY";
        private const string scenarioTemplateName = @"ReportTemplates\Scenario.arepa";
        private const string featureTemplateName = @"ReportTemplates\Feature.arepa";
        private const string testReportTemplateName = @"ReportTemplates\TestReport.arepa";

        private string projectFileName;
        private string testReportFile;
        private string scenarioTemplateFileName;  
        private string featureTemplateFileName;
        private string testReportTemplateFileName;
        private string testCategory;

        private bool errorDetected = false;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the project file name
        /// </summary>
        public string ProjectFileName
        {
            get { return projectFileName; }
            set { projectFileName = value; }
        }

        /// <summary>
        /// Get/Set the test report file name
        /// </summary>
        public string TestReportFile
        {
            get { return testReportFile; }
            set { testReportFile = value; }
        }

        /// <summary>
        /// Get the scenario report template file name
        /// </summary>
        public string ScenarioTemplateFileName
        {
            get { return scenarioTemplateFileName; }
        }

        /// <summary>
        /// Get the feature report template file name
        /// </summary>
        public string FeatureTemplateFileName
        {
            get { return featureTemplateFileName; }
        }

        /// <summary>
        /// Get the test report template file name
        /// </summary>
        public string TestReportTemplateFileName
        {
            get { return testReportTemplateFileName; }
        }

        /// <summary>
        /// Get/Set the project file name
        /// </summary>
        public string TestCategory
        {
            get { return testCategory; }
            set { testCategory = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">Arguments to inspect</param>
        public ProgramArguments(string[] args)
        {
            //Build the report template paths 
            scenarioTemplateFileName = AppDomain.CurrentDomain.BaseDirectory + scenarioTemplateName;
            featureTemplateFileName = AppDomain.CurrentDomain.BaseDirectory + featureTemplateName;
            testReportTemplateFileName = AppDomain.CurrentDomain.BaseDirectory + testReportTemplateName;

            
            FileManager fm = new FileManager();
            fm.MessageRaised += new EventHandler<MessageEventArgs>(fm_MessageRaised);

            if (args!=null)
            {
                int i = 0;
                foreach (string argument in args)
                {
                    switch (argument.ToUpperInvariant())
                    {
                        case projectDirArgument:
                            if(args.Length > i+1)
                                projectFileName = fm.GetProjectFileName(args[i + 1]);
                            break;
                        case testDirArgument:
                            if (args.Length > i + 1)
                            {
                                //MSTest
                                testReportFile = fm.GetMSTestFileName(args[i + 1]);
                                //TODO: Include here other testing tools
                            }
                            break;
                        case testCategoryArgument:
                                testCategory = args[i + 1];
                            break;
                    }
                    //Detect if errors were detected to avoid to continue
                    if (errorDetected)
                        break;
                    i++;
                }
            }

            //Confirms all arguments were populated and no errors were detected
            if ((string.IsNullOrEmpty(this.projectFileName) || string.IsNullOrEmpty(this.testReportFile)) && !errorDetected)
                Output.WriteMessage(new Message(MessageType.Error, ResourceLabel.StringErrorInvalidArguments, null));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for messages raised by the File Manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fm_MessageRaised(object sender, MessageEventArgs e)
        {
            //Detect if the error message raised is an error or not
            if (e.MsgRaised.TypeMessage == MessageType.Error)
                errorDetected = true;
            Output.WriteMessage(e.MsgRaised);
        }

        #endregion
    }
}
