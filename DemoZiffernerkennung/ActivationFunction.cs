using System;

namespace DemoZiffernerkennung
{
    public class ActivationFunction
    {
        public static double GetSigmoid(double x)
        {
            double sigmoid = 1.0 / (1.0 + Math.Exp(-x));
            return sigmoid;
        }

        public static double GetDerivative(double x)
        {
            double derivativeOfSigmoidOfX = x * (1.0 - x);
            return derivativeOfSigmoidOfX;
        }       
    }
}
