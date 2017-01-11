using System;
using System.Collections.Generic;
using System.Text;

namespace Task_2
{
    /// <summary>
    /// Exception for detection of incorrect path syntax with Regex.
    /// </summary>
    public class IncorrectPathSyntaxException : Exception
    {
        public IncorrectPathSyntaxException()
        {
        }
       
        public IncorrectPathSyntaxException(string message) : base(message)
        {
        } 

        public IncorrectPathSyntaxException(string message, Exception inner) : base(message, inner)
        {

        }
       
    }
}
