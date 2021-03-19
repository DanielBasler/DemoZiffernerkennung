using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace DemoZiffernerkennung
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int nFirstLayerNeurons = 0;
        private int nEpochMax = 10;
        private string AccuracyOfResult = "";
        private string TotalAccuracyOfNumbers = "";
        private int nIterationLoop = 50;
       
        double accuracySample = 0;
        double previousAccuracySample = 0;
        
        byte[] currentPixels;
        int currentColumn = 0;
        int currentRow = 0;

        NeuralNetwork neuralNet = new NeuralNetwork();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void tbTraining_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            neuralNet.errors.Clear();
            InitNewNeuralNetwork();
            TrainAllMnistImages(nEpochMax, true);
            teEpoch.Text = "Epoch: " + nEpochMax.ToString();
            teAccuracy.Text = AccuracyOfResult;
            Cursor = null;
            tbTest.IsEnabled = true;
        }

        private void tbTest_Click(object sender, RoutedEventArgs e)
        {
            neuralNet.errors.Clear();
            MessageBox.Show(TotalAccuracyOfNumbers, "Test Ergebnis");            
        }

        private void InitNewNeuralNetwork()
        {
            int neurons = Convert.ToInt32(teNeuron.Text.ToString());
            neuralNet.InitNeuralNetwork(neurons);
        }

        private void TrainAllMnistImages(int nEpochMax, bool accuracy)
        {
            int counter = 101;
            int trigger = 50;
            int totalImages = 0;            

            neuralNet.learningRate = double.Parse(teLearningRate.Text.ToString(), CultureInfo.InvariantCulture);

            for (int epoch = 0; epoch < nEpochMax; epoch++)
            {
                int n = 0;

                bool anyMoreNumbers = true;

                while (anyMoreNumbers)
                {
                    for (int expected = 0; expected < NeuralNetwork.outputLayer.outputNeurons; expected++)
                    {
                        bool useTestingDataset = false;                        
                        byte[] pixels = MnistImage.GetImageNumberPixels(expected, n, useTestingDataset);

                        currentPixels = pixels;

                        if (pixels != null)
                        { 
                            TrainNetOnNumber(pixels, expected);
                            currentColumn = MnistImage.GetCol(expected, n);
                            currentRow = MnistImage.GetRow(expected, n);                            
                        }
                        else
                        {                            
                            anyMoreNumbers = false;
                        }
                    }

                    n++;
                    totalImages++;                    

                    if (accuracy)
                    {
                        counter++;
                        if (counter > trigger)
                        {
                            AccuracyOfResult = Evaluation(nIterationLoop);
                            counter = 0;
                        }
                    }                    
                }
            }
        }       

        private void TrainNetOnNumber(byte[] pixels, int expected)
        {
            if (imageHasNumber(pixels))
            {
                NeuralNetwork.inputLayer.setInputs(pixels);
                neuralNet.TrainTheNeuralNetwork(1, expected);
            }
        }

        private string Evaluation(int nTestFactor)
        {            
            neuralNet.errors.Clear();
            int n = 0;
            int limit = nTestFactor;
            int rightAnalysis = 0;
            int analysis = 0;

            int[] rightNumbers = new int[10];
            int[] nNumbers = new int[10];

            bool anyMoreNumbers = true;

            for (int expected = 0; expected < NeuralNetwork.outputLayer.outputNeurons; expected++)
            {
                rightNumbers[expected] = 0;
                nNumbers[expected] = 0;
            }

            while(anyMoreNumbers)
            {
                anyMoreNumbers = false;

                for (int expected = 0; (expected < NeuralNetwork.outputLayer.outputNeurons); expected++)
                {
                    bool useTestingDataset = true;
                    
                    byte[] pixels = MnistImage.GetImageNumberPixels(expected, n, useTestingDataset);

                    currentPixels = pixels;

                    if (pixels != null)
                    {
                        anyMoreNumbers = true;

                        int estimated = 0;
                        double confidence = 0.0;

                        if (imageHasNumber(pixels))
                        {
                            if (TestNetOnNumber(pixels, expected, out estimated, out confidence))
                            {
                                rightAnalysis++;
                                rightNumbers[expected]++;
                            }
                            analysis++;
                            nNumbers[expected]++;
                        }                        
                    }
                }

                n++;               

                if (n > limit)
                {
                    break;
                }
            }

            double percentRight = (double)rightAnalysis / (double)analysis;

            previousAccuracySample = accuracySample;
            accuracySample = percentRight;

            string result = Math.Round(percentRight, 4).ToString();


            if (n > nTestFactor)
            {
                TotalAccuracyOfNumbers = "";                

                for (int i = 0; i < NeuralNetwork.outputLayer.outputNeurons; i++)
                {
                    double percentCorrect = ((double)rightNumbers[i] / (double)nNumbers[i])*100;
                    TotalAccuracyOfNumbers += "Konfidenz der Ziffer " + i.ToString() + " beträgt " + percentCorrect.ToString("0.00") + " %\r\n";                    
                }                
            }

            return result;
        }

        public bool TestNetOnNumber(byte[] pixels, int expected, out int estimated, out double confidence)
        {
            estimated = 0;
            confidence = 0.0;

            if (imageHasNumber(pixels))
            {
                NeuralNetwork.inputLayer.setInputs(pixels);
                return neuralNet.TestInference(expected, out estimated, out confidence);
            }

            return false;
        }

        public bool imageHasNumber(byte[] pixels)
        {
            int count = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] > 100)
                {
                    count++;
                }

                if (count > 10)
                {
                    return true;
                }
            }

            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbTest.IsEnabled = false;

            for (int i = 0; i < 10; i++)
            {
                MnistImage.trainingImages[i] = new MnistImage();
                MnistImage.testImages[i] = new MnistImage();

                MnistImage.LoadTrainingImage(i);
                MnistImage.LoadTestImage(i);
            }
        }        
    }
}
