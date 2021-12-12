using System.Diagnostics;

namespace ShuntingYardAlgorithm.Operands
{
    [DebuggerDisplay("{Value}")]
    internal class ConstantOperand : IOperand
    {
        public ConstantOperand(double value) => Value = value;

        public double Value { get; }
    }
}
