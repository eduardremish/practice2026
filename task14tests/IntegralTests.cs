using System;
using Xunit;
using task14;

namespace task14tests
{
    public class IntegralTests
    {
        [Fact]
        public void Test_ConstantFunction_SingleThread()
        {
            Assert.Equal(50, DefiniteIntegral.Solve(0, 10, x => 5, 1e-4, 1), 1e-3);
        }

        [Fact]
        public void Test_QuadraticFunction_FourThreads()
        {
            Assert.Equal(8.0 / 3.0, DefiniteIntegral.Solve(0, 2, x => x * x, 1e-5, 4), 1e-4);
        }

        [Fact]
        public void Test_LargeStep_ShouldStillWork()
        {

            double result = DefiniteIntegral.Solve(0, 1, x => x, 100.0, 2);
            Assert.True(result > 0);
        }
    }
}
