using System;
using System.Diagnostics;

namespace ShuntingYardAlgorithm.Operands
{
    [DebuggerDisplay("{Value}")]
    internal class VariableOperand : IOperand
    {
        private readonly Func<double> _value;

        public VariableOperand(Func<double> value) => _value = value;

        public double Value => _value();
    }
}
