using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuntingYardAlgorithm.Operands;
using System.Collections.Generic;

namespace ShuntingYardAlgorithmTests
{
    [TestClass]
    public class OperandTests
    {
        public static IEnumerable<object[]> OperandValues
        {
            get
            {
                yield return new object[] { 1 };
                yield return new object[] { 0 };
                yield return new object[] { -1 };
                yield return new object[] { double.MinValue };
                yield return new object[] { double.MaxValue };
            }
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues), DynamicDataSourceType.Property)]
        public void ConstantOperandTest(double value)
        {
            var coperand = new ConstantOperand(value);
            Assert.AreEqual(value, coperand.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues), DynamicDataSourceType.Property)]
        public void RelayOperandTest(double value)
        {
            var voperand = new RelayOperand(() => value);
            Assert.AreEqual(value, voperand.Value);
        }
    }
}
