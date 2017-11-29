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

namespace Arepa.Parser
{
    /// <summary>
    /// Represents a message to be printed
    /// </summary>
    public class Message : IMessage
    {
        #region Fields

        MessageType typeMessage;
        string messageDescription;
        ResourceFileManager resMan = new ResourceFileManager();

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set Message Type
        /// </summary>
        public MessageType TypeMessage
        {
            get { return typeMessage; }
            set { typeMessage = value; }
        }

        /// <summary>
        /// Get/Set the description of the message
        /// </summary>
        public string Description
        {
            get { return messageDescription; }
            set { messageDescription = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeOfMessage">Type of Message</param>
        /// <param name="description">Message description</param>
        /// <param name="label">Resource label to be used on message</param>
        /// <param name="arguments">Arguments to replace values on message</param>
        public Message(MessageType typeOfMessage, ResourceLabel label, string[] arguments)
        {
            typeMessage = typeOfMessage;
            string description = resMan.Resources.GetString(label.ToString());
            if(arguments==null)
                messageDescription = description;
            else
                messageDescription = string.Format(CultureInfo.CurrentCulture, description, arguments);
        }

        /// <summary>
        /// Constructor for simple messages
        /// </summary>
        /// <param name="typeOfMessage">Type of Message</param>
        /// <param name="description">Message description</param>
        public Message(MessageType typeOfMessage, string description)
        {
            typeMessage = typeOfMessage;
            messageDescription = description;
        }

        #endregion

    }

}
