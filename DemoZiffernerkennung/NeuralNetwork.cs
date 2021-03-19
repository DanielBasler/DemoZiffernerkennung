using System;
using System.Collections.Generic;

namespace DemoZiffernerkennung
{
    public class NeuralNetwork
    {
        public double learningRate = 0.01;       
        public static double learningRateInitial = 0.01;
        public static bool usePositiveWeights = false;
        public int nFirstHiddenLayerNeurons = 30;

        public List<InferenceError> errors = new List<InferenceError>();

        public static bool testing = false;
        public static int expected;

        public static Random rand = new Random(DateTime.Now.Millisecond);

        public static InputLayer inputLayer;
        public static HiddenLayer hiddenLayer;
        public static OutputLayer outputLayer;
        
        public NeuralNetwork()
        {
            
        }

        public void InitNeuralNetwork(int hiddenNeurons)
        {
            nFirstHiddenLayerNeurons = hiddenNeurons;
            Create();
        }

        private void Create()
        {
            inputLayer = new InputLayer();
            hiddenLayer = new HiddenLayer(nFirstHiddenLayerNeurons, this);
            outputLayer = new OutputLayer(hiddenLayer, this);
        }

        public void TrainTheNeuralNetwork(int nIterations, int expected)
        {
            SetExpected(expected);

            for (int n = 0; n < nIterations; n++)
            {
                Activate();
                Backpropagation();
            }
        }

        private void SetExpected(int exp)
        {
            expected = exp;

            for (int i = 0; i < outputLayer.neurons.Length; i++)
            {
                if (i == exp)
                {
                    outputLayer.neurons[i].expectedValue = 1.0;
                }
                else
                {
                    outputLayer.neurons[i].expectedValue = 0.0;
                }
            }
        }

        internal static double RandomWeight()
        {
            double span = 50000;
            int spanInt = (int)span;

            double magnitude = 10.0;

            if (usePositiveWeights)
            {
                return ((double)rand.Next(0, spanInt)) / (span * magnitude);
            }
            else
            {
                return ((double)(rand.Next(0, spanInt * 2) - spanInt)) / (span * magnitude);
            }
        }

        public void Activate()
        {
            hiddenLayer.Activate();
            outputLayer.Activate();
        }

        public void Backpropagation()
        {
            outputLayer.Backpropagation();
            hiddenLayer.Backpropagation();
        }

        internal bool TestInference(int expected, out int estimated, out double confidence)
        {
            SetExpected(expected);

            Activate();

            estimated = Analysis(out confidence);

            return (estimated == expected);
        }

        private int Analysis(out double confidence)
        {
            confidence = 0.0;
            double max = 0;

            int result = -1;

            for (int n = 0; n < outputLayer.outputNeurons; n++)
            {
                double s = outputLayer.neurons[n].sigmoidSum;

                if (s > max)
                {
                    confidence = s;
                    result = n;
                    max = s;
                }
            }

            return result;
        }
    }

    public class InferenceError
    {
        public int expected = 0;
        public int n = 0;
        public int estimate = 0;
        public double confidence = 0.0;

        public InferenceError(int e, int neuron, int est, double con)
        {
            expected = e;
            n = neuron;
            estimate = est;
            confidence = con;
        }
    }
}
