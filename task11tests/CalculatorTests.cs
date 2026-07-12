using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        private const string row = @"
        public class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }";

        [Fact]
        public void CreateCalculator_ShouldCompileAndExecuteAllMethods_WithoutReflection()
        {
            ICalculator calculator = ClassGenerator.CreateCalculator(row);

            Assert.NotNull(calculator);
            Assert.Equal(6, calculator.Add(1, 5));
            Assert.Equal(0, calculator.Minus(5, 5));
            Assert.Equal(100, calculator.Mul(100, 1));
            Assert.Equal(2, calculator.Div(2, 1));
        }

        [Fact]
        public void CreateCalculator_EmptyCode_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ClassGenerator.CreateCalculator(""));
        }

        [Fact]
        public void CreateCalculator_InvalidCode_ShouldThrowInvalidOperationException()
        {
            string badrow = @"
            public Calculator {
                public int Add(int a, int b) => a + b;
            }";

            Assert.Throws<InvalidOperationException>(() => ClassGenerator.CreateCalculator(badrow));
        }

        [Fact]
        public void CreateCalculator_ShouldHandleLargeNumbers()
        {
            ICalculator calculator = ClassGenerator.CreateCalculator(row);

            Assert.Equal(200000, calculator.Add(100000, 100000));
            Assert.Equal(0, calculator.Minus(1000000, 1000000));
            Assert.Equal(10000000, calculator.Mul(1000, 10000));
            Assert.Equal(1, calculator.Div(10000, 10000));
        }


    }
}
