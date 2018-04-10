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

namespace Arepa.Parser
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Replace case-insensitive values from a string
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="originalValue">Value to replace</param>
        /// <param name="newValue">New value</param>
        /// <returns>String with value replaces</returns>
        public static string ReplaceCaseInsensitiveValues(this string value, string originalValue, string newValue)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(originalValue))
            {
                //Replace values
                int i = value.IndexOf(originalValue, StringComparison.OrdinalIgnoreCase);
                //Replace form content all values found
                while (i != -1)
                {
                    value = value.Substring(0, i) + newValue + value.Substring((i + originalValue.Length), value.Length - (i + originalValue.Length));
                    i = value.IndexOf(originalValue, StringComparison.OrdinalIgnoreCase);
                }
            }

            return value;
        }

        /// <summary>
        /// Replace new lines by <br/> tags
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>Values replaces with <br/></returns>
        public static string ReplaceNewLineByBRTag(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder sb = new StringBuilder();
                string[] lines = value.Split(Environment.NewLine.ToCharArray());

                //Replace new lines by <br> (all except the last one). Add <strong> tag
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i < lines.Length - 1)
                        sb.Append(WrapGherkinWithStrongTag(lines[i].Trim()) + @"<br/>");
                    else
                        sb.Append(WrapGherkinWithStrongTag(lines[i].Trim()));
                }
                value = sb.ToString();
            }

            return value;
        }

        /// <summary>
        /// Converts a string into a HTML-encoded sring
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>string converted</returns>
        public static string HtmlEncode(this string value)
        {
            return System.Net.WebUtility.HtmlEncode(value);
        }

        private static string WrapGherkinWithStrongTag(string line)
        {
            string newLine = string.Empty;
            List<string> tags = new List<string>() { "As", "I", "So", "Given", "When", "Then", "And" };

            const string templateBold = "<strong>{0}</strong>";
            foreach (var tag in tags)
            {
                if (line.StartsWith(tag, StringComparison.InvariantCultureIgnoreCase))
                {
                    int place = line.IndexOf(tag, StringComparison.InvariantCultureIgnoreCase);
                    newLine = line.Remove(place, tag.Length).Insert(place, string.Format(templateBold, tag));
                    break;
                }
            }

            return newLine;
        }
    }

}
