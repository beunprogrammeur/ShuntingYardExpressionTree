using ShuntingYardAlgorithm.Operands;

namespace ShuntingYardAlgorithm.Operators
{
    public interface IOperator
    {
        IOperand Result { get; }
    }
}
