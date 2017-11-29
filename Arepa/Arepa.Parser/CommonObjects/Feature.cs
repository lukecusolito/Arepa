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
using System.Collections.ObjectModel;

namespace Arepa.Parser
{
    /// <summary>
    /// Represents a feature on memory
    /// </summary>
    public class Feature : IFeature
    {
        /// <summary>
        /// Feature Title defined on the XML documentation file
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// User story defined on the XML documentation file
        /// </summary>
        public string UserStory { get; set; }

        /// <summary>
        /// Content to be displayed on the test report
        /// </summary>
        public string ReportContent { get; set; }

        /// <summary>
        /// Scenarios assciated with this feature
        /// </summary>
        public Collection<Scenario> Scenarios { get; set; }

        /// <summary>
        /// Name of the object
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Feature()
        {
            this.Scenarios = new Collection<Scenario>();
        }
    }
}
