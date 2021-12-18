using System;
using System.Diagnostics;

namespace ShuntingYardAlgorithm.Operands
{
    [DebuggerDisplay("{Value}")]
    internal class RelayOperand : IOperand
    {
        private readonly Func<double> _value;

        public RelayOperand(Func<double> value) => _value = value;

        public double Value => _value();
    }
}