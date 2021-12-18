using ShuntingYardAlgorithm.Expression;
using System.Diagnostics;

namespace ShuntingYardAlgorithm.Operands
{
    [DebuggerDisplay("{Value}")]
    internal class VariableOperand : IOperand
    {
        private readonly IProperty _property;
        private readonly bool _invert;

        public VariableOperand(IProperty property, bool invert = false) 
        {
            _property = property;
            _invert   = invert;
        }

        public double Value => _invert ? -_property.Read() : _property.Read();
    }
}
