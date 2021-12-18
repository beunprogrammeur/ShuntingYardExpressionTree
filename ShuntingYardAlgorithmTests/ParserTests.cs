using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuntingYardAlgorithm;
using ShuntingYardAlgorithm.Exceptions;
using ShuntingYardAlgorithm.Expression;
using System.Collections.Generic;

namespace ShuntingYardAlgorithmTests
{
    [TestClass]
    public class ParserTests
    {
        public static IEnumerable<object[]> FormulaTestValues
        {
            get
            {
                yield return new object[] { "1 + 1",   2   };
                yield return new object[] { "1+1",     2   };
                yield return new object[] { "-1+2",    1   };
                yield return new object[] { "-1-2",   -3   };
                yield return new object[] { "-1-+2",  -3   };
                yield return new object[] { "-1+-2",  -3   };
                yield return new object[] { "1+2",     3   };
                yield return new object[] { "1-2",    -1   };
                yield return new object[] { "2*2",     4   };
                yield return new object[] { "2*3",     6   };
                yield return new object[] { "3*3.0",   9   };
                yield return new object[] { "2*3.5",   7   };
                yield return new object[] { "6/2",     3   };
                yield return new object[] { "6*6-6",   30  };
                yield return new object[] { "6-6*6",  -30  };
                yield return new object[] { "(6-6)*7", 0   };
                yield return new object[] { "(1+2)"  , 3   };
                yield return new object[] { "1+(2+3)", 6   };
                yield return new object[] { "3^4",     81  };
                yield return new object[] { "4^3",     64  };
                yield return new object[] { "5%2",     1   };
                yield return new object[] { "2%5",     2   };
            }
        }

        [TestMethod]
        [DynamicData(nameof(FormulaTestValues), DynamicDataSourceType.Property)]
        public void ParserParsesFormulasThatEvaluateCorrectly(string input, double expectedResult)
        {
            var parser  = new Parser();
            var formula = parser.ParseFormula(input);

            Assert.AreEqual(expectedResult, formula.Value);
        }

        [TestMethod]
        public void AddingDuplicateVariablesFailsGracefully()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable(new ValueProperty("test", 1)));
            Assert.IsFalse(parser.RegisterVariable(new ValueProperty("test", 1)));
            Assert.IsFalse(parser.RegisterVariable(new ValueProperty("test", 2)));
        }

        [TestMethod]
        public void AddingVariableIsCaseInsensitive()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable(new ValueProperty("test", 1)));
            Assert.IsFalse(parser.RegisterVariable(new ValueProperty("TEST", 3)));
        }

        [TestMethod]
        public void AddingVariableAppliesTrim()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable(new ValueProperty(" test", 1)));
            Assert.IsFalse(parser.RegisterVariable(new ValueProperty("test ", 3)));
        }

        [TestMethod]
        [DataRow("+")]
        [DataRow("1+")]
        [DataRow("(")]
        [DataRow(")")]
        [DataRow("(1+2")]
        [DataRow("1+2)")]
        public void PassingInvalidFormulaThrowsException(string formula)
        {
            var parser = new Parser();
            Assert.ThrowsException<ParseException>(() => parser.ParseFormula(formula));
        }

        [TestMethod]
        [DataRow("a", 10)]
        [DataRow("a*1", 10)]
        [DataRow("a*a", 100)]
        [DataRow("a+a", 20)]
        [DataRow("a-a", 0)]
        [DataRow("2*-a", -20)]
        [DataRow("-a", -10)]
        public void VariablesGetParsedCorrectly(string formula, double expectedAValue)
        {
            var parser = new Parser();
            parser.RegisterVariable(new ValueProperty("a", 10));
            var operand = parser.ParseFormula(formula);
            Assert.AreEqual(expectedAValue, operand.Value);
        }

        public static IEnumerable<object[]> ReadWritePropertiesExpresionTestValues
        {
            get
            {
                yield return new object[] { "a = 10", 10, 5 };
                yield return new object[] { "a = a*2", 20, 5 };
                yield return new object[] { "a = b*2", 10, 5 };
                yield return new object[] { "a = b", 5, 5 };
                yield return new object[] { "a = -b", -5, 5 };
                yield return new object[] { "a = 10; b = 11", 10, 11 };
                yield return new object[] { "a = 10\r b = 11", 10, 11 };
                yield return new object[] { "a = 10\n b = 11", 10, 11 };
                yield return new object[] { "a = 10\r\n b = 11", 10, 11 };
                yield return new object[] { "a = 15; b = a", 15, 15 };
            }
        }

        [TestMethod]
        [DynamicData(nameof(ReadWritePropertiesExpresionTestValues), DynamicDataSourceType.Property)]
        public void ParserParsesExpressionsWithProperties(string input, double aResult, double bResult)
        {
            var parser = new Parser();
            double a = 10;
            double b = 5;

            parser.RegisterVariable(new RelayProperty("a", () => a, (value) => a = value));
            parser.RegisterVariable(new RelayProperty("b", () => b, (value) => b = value));
            var expression = parser.ParseExpression(input);
            
            expression.Evaluate();

            Assert.AreEqual(aResult, a);
            Assert.AreEqual(bResult, b);
        }

        [TestMethod]
        public void ParserDoesntAllowMultipleAssignmentsPerLine()
        {
            var parser = new Parser();
            parser.RegisterVariable(new RelayProperty("readonly", () => 10));
            parser.RegisterVariable(new ValueProperty("rw"));
            Assert.ThrowsException<ParseException>(() => parser.ParseExpression("rw = readonly = readonly"));
        }

        [TestMethod]
        public void ParserRequiresAssigment()
        {
            var parser = new Parser();
            parser.RegisterVariable(new RelayProperty("readonly", () => 10));
            parser.RegisterVariable(new ValueProperty("rw"));

            Assert.ThrowsException<ParseException>(() => parser.ParseExpression("rw"));
        }
        
        [TestMethod]
        public void ParserThrowsOnWriteToReadonlyVariable()
        {
            var parser = new Parser();
            parser.RegisterVariable(new RelayProperty("readonly", () => 10));
            parser.RegisterVariable(new ValueProperty("rw"));
            parser.RegisterVariable(new ValueProperty("wo", 0, false, true));
            parser.RegisterVariable(new ValueProperty("ro", 0, true, false));

            Assert.ThrowsException<ParseException>(() => parser.ParseExpression("ro = 5"));
        }
        
        [TestMethod]
        public void ParserThrowsOnReadFromWriteonlyVariable()
        {
            var parser = new Parser();
            parser.RegisterVariable(new RelayProperty("readonly", () => 10));
            parser.RegisterVariable(new ValueProperty("rw"));
            parser.RegisterVariable(new ValueProperty("wo", 0, false, true));
            parser.RegisterVariable(new ValueProperty("ro", 0, true, false));

            Assert.ThrowsException<ParseException>(() => parser.ParseExpression("rw = wo"));
        }
    }
}
