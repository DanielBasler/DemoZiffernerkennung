namespace DemoZiffernerkennung
{
    public class InputLayer
    {
        public double[] inputs;
        public bool[] isDropout;

        public void setInputs(byte[] newInputs)
        {
            inputs = new double[newInputs.Length];

            if (isDropout == null || isDropout.Length != inputs.Length)
            {
                isDropout = new bool[inputs.Length];
            }

            for (int i = 0; i < newInputs.Length; i++)
            {
                inputs[i] = (double)newInputs[i] / 255.0;
            }           
        }
    }
}
