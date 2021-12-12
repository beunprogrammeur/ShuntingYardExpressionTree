using ShuntingYardAlgorithm.Operands;

namespace ShuntingYardAlgorithm.Operators
{
    internal abstract class BinaryOperator : IOperator
    {
        protected IOperand LeftOperand  { get; }
        protected IOperand RightOperand { get; }

        public BinaryOperator(IOperand leftOperand, IOperand rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public abstract IOperand Result { get; }
    }
}
