namespace DemoZiffernerkennung
{
    public class OutputLayer
    {
        public NeuralNetwork network;
        public HiddenLayer hiddenLayer;
        public int outputNeurons = 10;

        public OutputNeuron[] neurons;

        public OutputLayer(HiddenLayer h, NeuralNetwork neuralNetwork)
        {
            network = neuralNetwork;
            hiddenLayer = h;
            neurons = new OutputNeuron[outputNeurons];

            for (int i = 0; i < outputNeurons; i++)
            {
                neurons[i] = new OutputNeuron(this);
            }
        }

        public void Activate()
        {
            foreach (OutputNeuron n in neurons)
            {
                n.Activate();
            }
        }

        public void Backpropagation()
        {
            foreach (OutputNeuron n in neurons)
            {
                n.Backpropagation();
            }
        }
    }

    public class OutputNeuron
    {
        public OutputLayer outputLayer;

        public double sum = 0.0;
        public double sigmoidSum = 0.0;
        public double error = 0.0;

        public double[] weights;
        public double[] oldWeights;

        public double expectedValue = 0.0;

        public OutputNeuron(OutputLayer output)
        {
            outputLayer = output;
            weights = new double[outputLayer.hiddenLayer.hiddenNeurons];
            oldWeights = new double[outputLayer.hiddenLayer.hiddenNeurons];
            InitWeights();
        }

        private void InitWeights()
        {
            for (int y = 0; y < weights.Length; y++)
            {
                weights[y] = NeuralNetwork.RandomWeight();
            }
        }

        public void Activate()
        {
            sum = 0.0;

            for (int y = 0; y < weights.Length; y++)
            {
                if (!outputLayer.hiddenLayer.neurons[y].isDropout)
                {
                    sum += outputLayer.hiddenLayer.neurons[y].sigmoidSum * weights[y];
                }
            }

            sigmoidSum = ActivationFunction.GetSigmoid(sum);
        }

        public void Backpropagation()
        {
            CalculateError();

            int i = 0;
            foreach (HiddenNeuron n in outputLayer.hiddenLayer.neurons)
            {
                if (!n.isDropout)
                {
                    oldWeights[i] = weights[i];
                    weights[i] += (error * n.sigmoidSum) * outputLayer.network.learningRate;
                }

                i++;
            }
        }

        private void CalculateError()
        {
            error = ActivationFunction.GetDerivative(sigmoidSum) * (expectedValue - sigmoidSum);
        }
    }
}
