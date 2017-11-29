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
using System.Globalization;


[assembly: CLSCompliant(true)]
namespace Arepa.Parser
{
    public class ParserBase
    {
        #region Event handlers

        // An event that clients can use to be notified whenever the application requires it
        public event EventHandler<MessageEventArgs> MessageRaised;

        // Invoke the MessageRaised event
        protected virtual void OnMessageRaised(Message msgRaised)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = MessageRaised;
            if (temp != null)
                temp(this, new MessageEventArgs(msgRaised));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Manages the errors raised by the parser methods
        /// </summary>
        /// <param name="projectTested">Project Tested</param>
        /// <param name="errorMsg">Error message to raise</param>
        /// <param name="msgParameters">List of paramaters to associated to the error message</param>
        /// <returns>Project tested with the error flag on</returns>
        protected virtual Project ManageError(Project projectTested, ResourceLabel label, string[] msgParameters)
        {
            //Validate object before use it
            if (projectTested == null)
                return projectTested;

            projectTested.Error = true;          
            //Throw the proper error
            this.OnMessageRaised(new Message(MessageType.Error, label, msgParameters));

            return projectTested;
        }

        #endregion
    }
}
