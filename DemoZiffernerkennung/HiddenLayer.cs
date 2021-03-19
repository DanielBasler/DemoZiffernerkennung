namespace DemoZiffernerkennung
{
    public class HiddenLayer
    {
        public int hiddenNeurons = 0;

        public HiddenNeuron[] neurons;

        public NeuralNetwork network;

        public HiddenLayer(int n, NeuralNetwork neuralNetwork)
        {
            network = neuralNetwork;

            hiddenNeurons = n;

            neurons = new HiddenNeuron[hiddenNeurons];

            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new HiddenNeuron(i, this);
            }
        }

        public void Backpropagation()
        {
            foreach (HiddenNeuron n in neurons)
            {                
                n.Backpropagation();
            }
        }

        public void Activate()
        {
            foreach (HiddenNeuron n in neurons)
            {
                n.Activate();
            }
        }
    }

    public class HiddenNeuron
    {
        public bool isDropout = false;
        public int index = 0;
        public double error = 0;

        public double[] weights = new double[MnistImage.numberWidth * MnistImage.numberWidth];
        public double[] oldWeights = new double[MnistImage.numberWidth * MnistImage.numberWidth];

        public double sum = 0.0;
        public double sigmoidSum = 0.0;

        public HiddenLayer layer;

        public HiddenNeuron(int i, HiddenLayer h)
        {
            layer = h;
            index = i;
            InitWeights();
        }

        public void InitWeights()
        {
            weights = new double[MnistImage.numberWidth * MnistImage.numberWidth];

            for (int y = 0; y < weights.Length; y++)
            {
                weights[y] = NeuralNetwork.RandomWeight();
            }
        }

        public void Activate()
        {
            if (isDropout) return;
            sum = 0.0;

            for (int y = 0; y < weights.Length; y++)
            {
                if (!NeuralNetwork.inputLayer.isDropout[y])
                {
                    sum += NeuralNetwork.inputLayer.inputs[y] * weights[y];
                }
            }

            sigmoidSum = ActivationFunction.GetSigmoid(sum);
        }

        public void Backpropagation()
        {
            if (isDropout) return;
           
            double sumError = 0.0;

            foreach (OutputNeuron o in NeuralNetwork.outputLayer.neurons)
            {
                sumError += (o.error * o.oldWeights[index]);
            }

            error = ActivationFunction.GetDerivative(sigmoidSum) * sumError;

            for (int w = 0; w < weights.Length; w++)
            {
                weights[w] += (error * NeuralNetwork.inputLayer.inputs[w]) * layer.network.learningRate;
            }
        }
    }

}
