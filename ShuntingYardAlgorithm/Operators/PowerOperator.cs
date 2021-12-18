using ShuntingYardAlgorithm.Operands;
using System;

namespace ShuntingYardAlgorithm.Operators
{
    internal class PowerOperator : BinaryOperator
    {
        public PowerOperator(IOperand leftOperand, IOperand rightOperand) : base(leftOperand, rightOperand)
        {
        }

        public override IOperand Result => new RelayOperand(() => Math.Pow(LeftOperand.Value, RightOperand.Value));
    }
}
