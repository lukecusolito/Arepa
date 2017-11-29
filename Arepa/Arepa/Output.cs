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
using System.Globalization;
using System.Reflection;

namespace Arepa
{
    /// <summary>
    /// Writes messages on console
    /// </summary>
    internal static class Output
    {
        #region Fields

        static ResourceFileManager resx = new ResourceFileManager();

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if an error or warning was written during the lifecycle of the application
        /// </summary>
        public static bool ErrorWarningWasWritten { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Write the starting message on console
        /// </summary>
        /// <param name="startingTime">Application starting time</param>
        public static void WriteStartingMessage(DateTime startingTime)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, 
                resx.Resources.GetString(ResourceLabel.StringStartingMessage.ToString()), Assembly.GetExecutingAssembly().GetName().Version, 
                startingTime.ToLongTimeString()));
        }

        /// <summary>
        /// Write the ending message on console
        /// </summary>
        /// <param name="startingTime">Application ending time</param>
        public static void WriteEndingMessage(DateTime startingTime)
        {
            DateTime endingTime = DateTime.Now;
            TimeSpan ts = endingTime - startingTime;
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, resx.Resources.GetString(ResourceLabel.StringFinishingMessage.ToString()), 
                Assembly.GetExecutingAssembly().GetName().Version,
                endingTime.ToLongTimeString(), ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds));
        }

        /// <summary>
        /// Writes the message on console
        /// </summary>
        /// <param name="messageToWrite">Message to write</param>
        public static void WriteMessage(Message messageToWrite)
        {
            switch (messageToWrite.TypeMessage)
            {
                case MessageType.Error:
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, resx.Resources.GetString(ResourceLabel.StringTemplateError.ToString()), messageToWrite.Description));
                    ErrorWarningWasWritten = true;
                    break;
                case MessageType.Warning:
                    ErrorWarningWasWritten = true;
                    break;
                case MessageType.Information:
                    Console.WriteLine(String.Format(CultureInfo.CurrentCulture, resx.Resources.GetString(ResourceLabel.StringTemplateInformation.ToString()), messageToWrite.Description));
                    break;
            }
        }

        #endregion
    }
}
