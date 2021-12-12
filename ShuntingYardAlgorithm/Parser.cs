using ShuntingYardAlgorithm.Operands;
using ShuntingYardAlgorithm.Operators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShuntingYardAlgorithm
{
    public interface IParser
    {
        bool RegisterVariable(string name, Func<double> variable);
        IOperand Parse(string formula);
    }
    public class Parser : IParser
    {
        enum TokenClass
        {
            Number,
            Variable,
            Operator,
            BracketOpen,
            BracketClose
        }
        
        private static readonly Regex _tokenRegex = new Regex(@"([-+]?[0-9.]+|[a-zA-Z_]+|[\+\-\*\/\(\\%\^)])", RegexOptions.Compiled);

        private readonly CultureInfo _culture;
        private readonly OperatorFactoryFactory _operatorSource;
        private readonly Dictionary<string, Func<double>> _variables;

        /// <summary>
        /// Instantiates the ShuntingYardParser with invariant culture
        /// </summary>
        public Parser() : this(CultureInfo.InvariantCulture) 
        {
        }

        public Parser(CultureInfo culture)
        {
            _variables      = new Dictionary<string, Func<double>>();
            _operatorSource = new OperatorFactoryFactory();
            _culture        = culture;
        }

        public IOperand Parse(string formula)
        {
            try
            {
                var words = Tokenize(formula);
                if (words == null) return null;

                var operators = new Stack<IOperatorFactory>();
                var operands  = new Stack<IOperand>();

                foreach (var word in words)
                {
                    switch (ClassifyToken(word))
                    {
                        case TokenClass.Number: operands.Push(new ConstantOperand(double.Parse(word, NumberStyles.Any, _culture))); break;
                        case TokenClass.Variable: operands.Push(GetVariable(word)); break;
                        case TokenClass.Operator: Operate(word, operators, operands); break;
                        case TokenClass.BracketOpen: operators.Push(_operatorSource.Create(word[0])); break;
                        case TokenClass.BracketClose: HandleGroupedOperandsAndOperators(operators, operands); break;
                    }
                }

                while (operators.Count > 0) Operate(operators, operands);
                return operands.Peek();
            }
            catch { return null; }
        }

        private static void Operate(Stack<IOperatorFactory> operators, Stack<IOperand> operands)
        {
            var right = operands.Pop();
            var left  = operands.Pop();
            var op    = operators.Pop();
            operands.Push(op.Create(left, right).Result);
        }

        private void Operate(string word, Stack<IOperatorFactory> operators, Stack<IOperand> operands)
        {
            var op = _operatorSource.Create(word[0]);
            while(operators.Count > 0 && operators.Peek().Precedence >= op.Precedence)
            {
                Operate(operators, operands);
            }
            operators.Push(op);
        }

        private static void HandleGroupedOperandsAndOperators(Stack<IOperatorFactory> operators, Stack<IOperand> operands)
        {
            while(operators.Count > 0 && operators.Peek().Type != OperatorType.GroupEnd && operators.Peek().Type != OperatorType.GroupStart && operands.Count > 1)
            {
                Operate(operators, operands);
            }
            operators.Pop();
        }

        private VariableOperand GetVariable(string name)
        {
            name = name.Trim().ToUpper();
            if(_variables.ContainsKey(name))
            {
                return new VariableOperand(_variables[name]);
            }
            throw new ArgumentException($"variable '{name}' is not registered");
        }

        /// <summary>
        /// Groups characters in the formula string into usable tokens.
        /// for instance: '1' '.' '0' would be one token: "1.0"
        /// </summary>
        /// <param name="formula">The input string</param>
        /// <returns>An array of tokens / words</returns>
        private static string[] Tokenize(string formula)
        {
            var matches = _tokenRegex.Matches(formula);
            var words = new List<string>();
            
            int count = matches.Count;
            for(int i = 0; i < count; i++)
            {
                if (matches[i].Success) words.Add(matches[i].Groups[1].Value);
            }

            return SanitizeTokens(words).ToArray();
        }

        private static IEnumerable<string> SanitizeTokens(IEnumerable<string> tokens)
        {
            bool lastWasNumber = false;
            foreach(string token in tokens)
            {
                if(double.TryParse(token, out double number))
                {
                    if (lastWasNumber) yield return "+"; // insert + operator when double numbers show up
                    lastWasNumber = true;
                }
                else 
                { 
                    lastWasNumber = false; 
                }

                yield return token;
            }
        }

        private TokenClass ClassifyToken(string token)
        {
            if(token.Length == 1)
            {
                if (token == "(")                         return TokenClass.BracketOpen;
                if (token == ")")                         return TokenClass.BracketClose;
                if (_operatorSource.IsOperator(token[0])) return TokenClass.Operator;
            }

            if (double.TryParse(token, out double result)) return TokenClass.Number;
            return TokenClass.Variable;
        }

        public bool RegisterVariable(string name, Func<double> variable)
        {
            name = name.Trim().ToUpper();
            
            if (_variables.ContainsKey(name)) return false;
            
            _variables.Add(name, variable);
            
            return true;
        }
    }
}
