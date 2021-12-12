using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuntingYardAlgorithm;
using System.Collections.Generic;

namespace ShuntingYardAlgorithmTests
{
    [TestClass]
    public class ParserTests
    {
        public static IEnumerable<object[]> TestValues
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
        [DynamicData(nameof(TestValues), DynamicDataSourceType.Property)]
        public void ParserParsesFormulasThatEvaluateCorrectly(string input, double expectedResult)
        {
            var parser  = new Parser();
            var formula = parser.Parse(input);

            Assert.AreEqual(expectedResult, formula.Value);
        }

        [TestMethod]
        public void AddingDuplicateVariablesFailsGracefully()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable("test", () => 1));
            Assert.IsFalse(parser.RegisterVariable("test", () => 1));
            Assert.IsFalse(parser.RegisterVariable("test", () => 3));
        }

        [TestMethod]
        public void AddingVariableIsCaseInsensitive()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable("test", () => 1));
            Assert.IsFalse(parser.RegisterVariable("TEST", () => 3));
        }

        [TestMethod]
        public void AddingVariableAppliesTrim()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.RegisterVariable(" test", () => 1));
            Assert.IsFalse(parser.RegisterVariable("test ", () => 3));
        }

        [TestMethod]
        [DataRow("+")]
        [DataRow("1+")]
        [DataRow("(")]
        [DataRow(")")]
        [DataRow("(1+2")]
        [DataRow("1+2)")]
        public void PassingInvalidFormulaReturnsNull(string formula)
        {
            var parser = new Parser();
            Assert.IsNull(parser.Parse(formula));
        }
    }
}
