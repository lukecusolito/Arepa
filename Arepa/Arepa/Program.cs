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
using Arepa.Parser;


[assembly: CLSCompliant(true)]
namespace Arepa
{
    /// <summary>
    /// Console
    /// </summary>
    class Program
    {
        #region Fields

        //Application starting time
        static DateTime startingTime = DateTime.Now;

        #endregion

        /// <summary>
        /// Entry Point
        /// </summary>
        /// <param name="args">DocumentationFileName TestReportFileName</param>
        static void Main(string[] args)
        {
            try
            {
                //Display Starting Message
                Output.WriteStartingMessage(startingTime);

                ProgramArguments pArgs = new ProgramArguments(args);
                //Detect if all arguments were populated to continue
                if (string.IsNullOrEmpty(pArgs.ProjectFileName) || string.IsNullOrEmpty(pArgs.TestReportFile))
                {
                    Output.WriteEndingMessage(startingTime);
                    return;
                }

                Project projectTested = new Project();
                FileParser parser = new FileParser();
                parser.MessageRaised += new EventHandler<MessageEventArgs>(parser_MessageRaised);
                //Parse the project file
                projectTested = parser.ParseProjectFile(projectTested, pArgs.ProjectFileName);
                if (projectTested.Error)
                {
                    Output.WriteEndingMessage(startingTime);
                    return;
                }
                //Parse the documentation file into objects on memory
                //TODO: Refactor two parameters to one
                projectTested = parser.ParseDocumentationFile(projectTested, projectTested.DocumentationFile);
                if (projectTested.Error)
                {
                    Output.WriteEndingMessage(startingTime);
                    return;
                }
                //Parse the MSTest file into object on memory
                projectTested = parser.ParseMSTestFile(projectTested, pArgs.TestReportFile, pArgs.TestCategory);
                if (projectTested.Error)
                {
                    Output.WriteEndingMessage(startingTime);
                    return;
                }
                //Generates the html report
                Report reportManager = new Report(projectTested);
                reportManager.MessageRaised += new EventHandler<MessageEventArgs>(parser_MessageRaised);

                string reportContent = reportManager.PrepareReportContent(pArgs.ScenarioTemplateFileName, pArgs.FeatureTemplateFileName, pArgs.TestReportTemplateFileName);
                if (projectTested.Error)
                {
                    Output.WriteEndingMessage(startingTime);
                    return;
                }
                string reportFileName = reportManager.SaveReportOnDisc(reportContent);
                
                //Print all ok on screen if no errors or warnings were written
                if (!Output.ErrorWarningWasWritten)
                    Output.WriteMessage(new Message(MessageType.Information, ResourceLabel.StringInformationNoErrorOrSuggestion, null));

                //Print Report Name on screen
                Output.WriteMessage(new Message(MessageType.Information, ResourceLabel.StringInformationReportGeneratedSuccessfully, new string[] { reportFileName }));
                Output.WriteEndingMessage(startingTime);
            }
            catch (Exception ex)
            {
                //Write unhandled error out on console
                Output.WriteMessage(new Message(MessageType.Error, ex.Message));
                Output.WriteEndingMessage(startingTime);
            }
        }

        #region Methods

        /// <summary>
        /// Event Handler responsible of write message on the console
        /// </summary>
        /// <param name="sender">Object who raise the event</param>
        /// <param name="e">Customer MessageEventArgs with the message to write</param>
        static void parser_MessageRaised(object sender, MessageEventArgs e)
        {
            //Write the message on console
            Output.WriteMessage(e.MsgRaised);
        }

        #endregion
    }
}
