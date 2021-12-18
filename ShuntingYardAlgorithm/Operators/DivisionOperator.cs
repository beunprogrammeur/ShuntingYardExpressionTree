using ShuntingYardAlgorithm.Operands;

namespace ShuntingYardAlgorithm.Operators
{
    internal class DivisionOperator : BinaryOperator
    {
        public DivisionOperator(IOperand leftOperand, IOperand rightOperand) : base(leftOperand, rightOperand)
        {
        }

        public override IOperand Result => new RelayOperand(() => LeftOperand.Value / RightOperand.Value);
    }
}
