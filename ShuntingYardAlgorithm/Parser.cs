using ShuntingYardAlgorithm.Exceptions;
using ShuntingYardAlgorithm.Expression;
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
        bool RegisterVariable(IProperty property);
        IOperand ParseFormula(string formula);
        IExpression ParseExpression(string formula);
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
        
        private static readonly Regex _tokenRegex = new Regex(@"([-+]?[0-9.]+|[-+]?[a-zA-Z_]+|[\+\-\*\/\(\\%\^)])", RegexOptions.Compiled);

        private readonly CultureInfo _culture;
        private readonly OperatorFactoryFactory _operatorSource;
        private readonly Dictionary<string, IProperty> _variables;

        /// <summary>
        /// Instantiates the ShuntingYardParser with invariant culture
        /// </summary>
        public Parser() : this(CultureInfo.InvariantCulture) 
        {
        }

        public Parser(CultureInfo culture)
        {
            _variables      = new Dictionary<string, IProperty>();
            _operatorSource = new OperatorFactoryFactory();
            _culture        = culture;
        }

        public IOperand ParseFormula(string formula)
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
                    case TokenClass.Variable: AddVariable(operands, word); break;
                    case TokenClass.Operator: Operate(word, operators, operands); break;
                    case TokenClass.BracketOpen: operators.Push(_operatorSource.Create(word[0])); break;
                    case TokenClass.BracketClose: HandleGroupedOperandsAndOperators(operators, operands); break;
                }
            }

            while (operators.Count > 0) Operate(operators, operands);
            return operands.Peek();
        }

        private void AddVariable(Stack<IOperand> operands, string word)
        {
            bool invert = false;
            var property = GetVariable(word, ref invert);
            if (!property.CanRead) throw new ParseException($"trying to read from variable '{word}' which has disabled reading");
            operands.Push(new VariableOperand(property, invert));
        }

        private static void Operate(Stack<IOperatorFactory> operators, Stack<IOperand> operands)
        {
            if (operands.Count < 2 || operators.Count == 0) throw new ParseException("invalid formula");
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
            if (operators.Count == 0) throw new ParseException("invalid formula, operator count mismatch");
            operators.Pop();
        }

        private IProperty GetVariable(string name, ref bool invert)
        {
            var property = TryGetVariable(name, ref invert);
            if (property != null) return property;
            throw new ArgumentException($"variable '{name}' is not registered");
        }

        private IProperty TryGetVariable(string name, ref bool invert)
        {
            name = name.Trim().ToUpper();
            invert = name.StartsWith("-");
            bool skipFirstCharacter = invert || name.StartsWith("+");
            IProperty property = null;

            if (skipFirstCharacter) name = name.Substring(1, name.Length - 1);

            if (_variables.ContainsKey(name))
            {
                property = _variables[name];
            }

            return property;
        }

        /// <summary>
        /// Groups characters in the formula string into usable tokens.
        /// for instance: '1' '.' '0' would be one token: "1.0"
        /// </summary>
        /// <param name="formula">The input string</param>
        /// <returns>An array of tokens / words</returns>
        private string[] Tokenize(string formula)
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

        private IEnumerable<string> SanitizeTokens(IEnumerable<string> tokens)
        {
            TokenClass oldClass = TokenClass.Operator;
            foreach(string token in tokens)
            {
                var newClass = ClassifyToken(token);

                if((oldClass == TokenClass.Number || oldClass == TokenClass.Variable) && (newClass == TokenClass.Number || newClass == TokenClass.Variable))
                {
                    yield return "+"; // insert + operator when double numbers show up
                    
                }
                oldClass = newClass;
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

        public bool RegisterVariable(IProperty property)
        {
            var name = property.Name.Trim().ToUpper();
            
            if (_variables.ContainsKey(name)) return false;
            
            _variables.Add(name, property);
            
            return true;
        }

        /// <summary>
        /// Parses expressions that assign a value to one or more properties.
        /// example:
        /// assume you have registered a read/writable property called 'a'
        /// assume you have registered a readable property called 'unix_time_stamp'
        /// var expression = ParseExpression("a = unix_time_stamp; a = a % 10");
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public IExpression ParseExpression(string formula)
        {
            var expression = new MultiExpression();

            var lines = formula.Split(new string[] { ";", "\r\n", "\n\r", "\r", "\n" }, StringSplitOptions.None);
            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                string[] parts = line.Split('=');

                if (line.IndexOf('=') < 0) throw new ParseException($"missing equals operator '=' on line {i+1}");
                if(parts.Length != 2) throw new ParseException($"equals operator must be given only once in every assignment");

                string propertyName = parts[0];
                bool invert = false; // not used in this case
                var property = TryGetVariable(propertyName, ref invert);

                if (property == null) throw new ParseException($"variable '{propertyName}' on line {i+1} not found in registry");
                if (!property.CanWrite) throw new ParseException($"variable '{propertyName}' on line {i + 1} is readonly");

                try
                {
                    var operand = ParseFormula(parts[1]);
                    expression.Add(() => property.Write(operand.Value));
                }
                catch(ParseException e)
                {
                    throw new ParseException($"{e.Message} '{parts[1]}' on line {i + 1}");
                }
            }
            return expression;
        }
    }
}
