﻿using ShuntingYardAlgorithm.Operands;

namespace ShuntingYardAlgorithm.Operators
{
    internal class MinusOperator : BinaryOperator
    {
        public MinusOperator(IOperand leftOperand, IOperand rightOperand) : base(leftOperand, rightOperand)
        {
        }

        public override IOperand Result => new VariableOperand(() => LeftOperand.Value - RightOperand.Value);
    }
}