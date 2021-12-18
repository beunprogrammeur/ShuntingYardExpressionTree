using System;
using System.Collections.Generic;

namespace ShuntingYardAlgorithm.Expression
{
    internal class MultiExpression : IExpression
    {
        private readonly List<Action> _expressions;
        public MultiExpression()
        {
            _expressions = new List<Action>();
        }

        public void Add(Action expression)
        {
            _expressions.Add(expression);
        }

        public void Evaluate()
        {
            foreach(var expression in _expressions)
            {
                expression();
            }
        }
    }
}
