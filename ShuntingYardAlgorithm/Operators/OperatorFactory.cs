using ShuntingYardAlgorithm.Operands;
using System.Collections.Generic;
using System.Linq;

namespace ShuntingYardAlgorithm.Operators
{
    internal enum OperatorType
    {
        Unkown,
        Plus,
        Minus,
        Multiplication,
        Division,
        Modulus,
        Power,
        GroupStart,
        GroupEnd,
    }

    internal class OperatorFactoryFactory
    {
        private static readonly Dictionary<OperatorType, IOperatorFactory> _factories;

        static OperatorFactoryFactory()
        {
            _factories = new Dictionary<OperatorType, IOperatorFactory>
            {
                { OperatorType.Plus,           new PlusOperatorFactory()           },
                { OperatorType.Minus,          new MinusOperatorFactory()          },
                { OperatorType.Multiplication, new MultiplicationOperatorFactory() },
                { OperatorType.Division,       new DivisionOperatorFactory()       },
                { OperatorType.Modulus,        new ModulusOperatorFactory()        },
                { OperatorType.Power,          new PowerOperatorFactory()          },
                { OperatorType.GroupStart,     new GroupStartOperatorFactory()     },
                { OperatorType.GroupEnd,       new GroupEndOperatorFactory()       },
            };
        }

        internal IOperatorFactory Create(OperatorType type)  => _factories[type];
        internal IOperatorFactory Create(char character)     => _factories.Values.FirstOrDefault(x => x.Character == character);
        internal bool             IsOperator(char character) => _factories.Values.Any(x => x.Character == character);
    }

    internal interface IOperatorFactory
    {
        int Precedence    { get; }
        char Character    { get; }
        OperatorType Type { get; }

        IOperator Create(IOperand left, IOperand right);
    }

    internal abstract class OperatorFactory : IOperatorFactory
    {
        public int  Precedence   { get; }
        public char Character    { get; }
        public OperatorType Type { get; }

        protected OperatorFactory(int precedence, char chararcter, OperatorType type)
        {
            Precedence = precedence;
            Character = chararcter;
            Type = type;
        }
        public abstract IOperator Create(IOperand left, IOperand right);
    }

    internal class PlusOperatorFactory : OperatorFactory
    {
        public PlusOperatorFactory() : base(1, '+', OperatorType.Plus) { }

        public override IOperator Create(IOperand left, IOperand right) => new PlusOperator(left, right);
    }

    internal class MinusOperatorFactory : OperatorFactory
    {
        public MinusOperatorFactory() : base(1, '-', OperatorType.Minus) { }

        public override IOperator Create(IOperand left, IOperand right) => new MinusOperator(left, right);
    }

    internal class MultiplicationOperatorFactory : OperatorFactory
    {
        public MultiplicationOperatorFactory() : base(2, '*', OperatorType.Multiplication) { }

        public override IOperator Create(IOperand left, IOperand right) => new MultiplicationOperator(left, right);
    }

    internal class DivisionOperatorFactory : OperatorFactory
    {
        public DivisionOperatorFactory() : base(2, '/', OperatorType.Division) { }

        public override IOperator Create(IOperand left, IOperand right) => new DivisionOperator(left, right);
    }

    internal class ModulusOperatorFactory : OperatorFactory
    {
        public ModulusOperatorFactory() : base(2, '%', OperatorType.Modulus) { }

        public override IOperator Create(IOperand left, IOperand right) => new ModulusOperator(left, right);
    }

    internal class PowerOperatorFactory : OperatorFactory
    {
        public PowerOperatorFactory() : base(2, '^', OperatorType.Power) { }

        public override IOperator Create(IOperand left, IOperand right) => new PowerOperator(left, right);
    }

    internal class GroupStartOperatorFactory : OperatorFactory
    {
        public GroupStartOperatorFactory() : base(0, '(', OperatorType.GroupStart) { }

        public override IOperator Create(IOperand left, IOperand right) => null;
    }

    internal class GroupEndOperatorFactory : OperatorFactory
    {
        public GroupEndOperatorFactory() : base(0, ')', OperatorType.GroupEnd) { }

        public override IOperator Create(IOperand left, IOperand right) => null;
    }
}