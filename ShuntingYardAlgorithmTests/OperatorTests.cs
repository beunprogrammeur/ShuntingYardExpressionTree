using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuntingYardAlgorithm.Operands;
using ShuntingYardAlgorithm.Operators;
using System;
using System.Collections.Generic;

namespace ShuntingYardAlgorithmTests
{
    [TestClass]
    public class OperatorTests
    {
        public static IEnumerable<object[]> OperandValues2
        {
            get
            {
                yield return new object[] { 1, 1 };
                yield return new object[] { 1, 0 };
                yield return new object[] { 1, -1 };
                yield return new object[] { double.MinValue, double.MinValue };
                yield return new object[] { double.MaxValue, double.MaxValue };
            }
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void PlusOperatorTest(double loperand, double roperand)
        {
            var op = new PlusOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(loperand + roperand, op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void MinusOperatorTest(double loperand, double roperand)
        {
            var op = new MinusOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(loperand - roperand, op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void MultiplyOperatorTest(double loperand, double roperand)
        {
            var op = new MultiplicationOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(loperand * roperand, op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void DivideOperatorTest(double loperand, double roperand)
        {
            var op = new DivisionOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(loperand / roperand, op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void PowerOperatorTest(double loperand, double roperand)
        {
            var op = new PowerOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(Math.Pow(loperand, roperand), op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues2), DynamicDataSourceType.Property)]
        public void ModulusOperatorTest(double loperand, double roperand)
        {
            var op = new ModulusOperator(new ConstantOperand(loperand), new ConstantOperand(roperand));
            Assert.AreEqual(loperand % roperand, op.Result.Value);
        }

        public static IEnumerable<object[]> OperandValues3
        {
            get
            {
                yield return new object[] { 1, 2, 3 };
                yield return new object[] { 0, 1, -1 };
                yield return new object[] { 0, double.MaxValue, double.MinValue };
            }
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues3), DynamicDataSourceType.Property)]
        public void NestedPlusOperands(double left, double middle, double right)
        {
            var op = new PlusOperator(new PlusOperator(new ConstantOperand(left), new ConstantOperand(middle)).Result, new ConstantOperand(right));
            Assert.AreEqual(left + middle + right, op.Result.Value);
        }

        [TestMethod]
        [DynamicData(nameof(OperandValues3), DynamicDataSourceType.Property)]
        public void NestedMixedOperands(double left, double middle, double right)
        {
            var op = new PlusOperator(new MinusOperator(new ConstantOperand(left), new ConstantOperand(middle)).Result, new ConstantOperand(right));
            Assert.AreEqual((left - middle) + right, op.Result.Value);
        }
    }
}
