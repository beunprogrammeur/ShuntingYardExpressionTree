using System;
using System.Collections.Generic;
using System.Text;

namespace ShuntingYardAlgorithm.Exceptions
{
    internal class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
