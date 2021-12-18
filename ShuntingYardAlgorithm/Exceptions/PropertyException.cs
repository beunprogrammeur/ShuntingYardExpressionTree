using System;

namespace ShuntingYardAlgorithm.Exceptions
{
    internal class PropertyException : Exception
    {
        public PropertyException(string message) : base(message)
        {
        }
    }
}
