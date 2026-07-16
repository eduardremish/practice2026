using System;
using System.Threading;

namespace task14
{
    public class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsCount)
        {
            double sum = 0.0;
            using var barrier = new Barrier(threadsCount + 1);
            double width = (b - a) / threadsCount;
            Thread[] threads = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                int id = i;
                threads[i] = new Thread(() =>
                {
                    double start = a + id * width;
                    double end = (id == threadsCount - 1) ? b : start + width;
                    double localsum = 0.0;

                    int steps = Math.Max(1, (int)Math.Round((end - start) / step));
                    double actualStep = (end - start) / steps;

                    for (int j = 0; j < steps; j++)
                    {
                        double x1 = start + j * actualStep;
                        double x2 = x1 + actualStep;
                        localsum += (function(x1) + function(x2)) / 2.0 * actualStep;
                    }

                    SafeAdd(ref sum, localsum);
                    barrier.SignalAndWait();
                });

                threads[i].Start();
            }

            barrier.SignalAndWait();
            return sum;
        }

        private static void SafeAdd(ref double target, double value)
        {
            double initial, newValue;
            do
            {
                initial = target;
                newValue = initial + value;
            }
            while (Interlocked.CompareExchange(ref target, newValue, initial) != initial);
        }

        public static double SolveSingleThreaded(double a, double b, Func<double, double> function, double step)
        {
            double result = 0.0;
            int steps = Math.Max(1, (int)Math.Round((b - a) / step));
            double actualStep = (b - a) / steps;

            for (int i = 0; i < steps; i++)
            {
                double x1 = a + i * actualStep;
                double x2 = x1 + actualStep;
                result += (function(x1) + function(x2)) / 2.0 * actualStep;
            }

            return result;
        }
    }
}
