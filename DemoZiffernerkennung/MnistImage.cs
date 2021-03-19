using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DemoZiffernerkennung
{
    public class MnistImage
    {
        public static int numberWidth = 28;
        public static int nTypes = 10;

        static Random random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second);

        public static MnistImage[] trainingImages = new MnistImage[10];
        public static MnistImage[] testImages = new MnistImage[10];

        public Bitmap image = null;
        public byte[] imageBytes;

        List<byte[]> pixelRows = null;

        public static void LoadTrainingImage(int n)
        {
            trainingImages[n].image = new Bitmap(@"c:/Temp/mnist_" + n.ToString() + ".jpg");
            trainingImages[n].imageBytes = ByteArrayFromImage(trainingImages[n].image);
        }

        public static void LoadTestImage(int n)
        {
            testImages[n].image = new Bitmap(@"c:/Temp/mnistTest_" + n.ToString() + ".jpg");
            testImages[n].imageBytes = ByteArrayFromImage(testImages[n].image);
        }

        public static byte[] ByteArrayFromImage(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;

            int numBytes = data.Stride * bmp.Height;
            byte[] image_bytes = new byte[numBytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, image_bytes, 0, numBytes);

            bmp.UnlockBits(data);

            return image_bytes;
        }

        public static byte[] GetNumberPixels(int column, int row, List<byte[]> pixels)
        {
            byte[] digitPixels = new byte[numberWidth * numberWidth];

            int index = 0;

            for (int y = row * numberWidth; y < (row * numberWidth) + numberWidth; y++)
            {
                for (int x = column * numberWidth; x < (column * numberWidth) + numberWidth; x++)
                {
                    digitPixels[index] = pixels[y][x];
                    index++;
                }
            }

            return digitPixels;
        }

        public static Bitmap GetImage(int expected, bool testing)
        {
            if (testing)
            {
                return testImages[expected].image;
            }
            else
            {
                return trainingImages[expected].image;
            }
        }

        public static byte[] GetImageNumberPixels(int expected, int column, int row, bool testing)
        {
            Bitmap image = GetImage(expected, testing);         

            LoadPixelRows(expected, testing);

            return GetNumberPixels(column, row,
                testing ? testImages[expected].pixelRows : trainingImages[expected].pixelRows
                );
        }       

        public static void LoadPixelRows(int Expected, bool testing)
        {
            if (testing)
            {
                byte[] imageBytes = testImages[Expected].imageBytes;

                Bitmap image = testImages[Expected].image;

                if (testImages[Expected].pixelRows == null)
                {
                    testImages[Expected].pixelRows = new List<byte[]>();

                    for (int i = 0; i < image.Height; i++)
                    {
                        int index = i * image.Width;
                        byte[] rowBytes = new byte[image.Width];
                        for (int w = 0; w < image.Width; w++)
                        {
                            rowBytes[w] = imageBytes[index + w];
                        }
                        
                        testImages[Expected].pixelRows.Add(rowBytes);
                    }
                }
            }
            else
            {
                byte[] imageBytes = trainingImages[Expected].imageBytes;

                Bitmap image = trainingImages[Expected].image;

                if (trainingImages[Expected].pixelRows == null)
                {
                    trainingImages[Expected].pixelRows = new List<byte[]>();

                    for (int i = 0; i < image.Height; i++)
                    {
                        int index = i * image.Width;
                        byte[] rowBytes = new byte[image.Width];
                        for (int w = 0; w < image.Width; w++)
                        {
                            rowBytes[w] = imageBytes[index + w];
                        }
                        
                        trainingImages[Expected].pixelRows.Add(rowBytes);
                    }
                }
            }
        }

        public static int GetRow(int expected, int index)
        {
            Bitmap image = trainingImages[expected].image;

            int nColumns = image.Width / numberWidth;
            int row = index / nColumns;
            
            return row;
        }

        public static int GetCol(int expected, int index)
        {
            Bitmap image = trainingImages[expected].image;

            int nColumns = image.Width / numberWidth;
            int column = index % nColumns;
            
            return column;
        }

        public static byte[] GetImageNumberPixels(int expected, int index, bool testing)
        {
            Bitmap image = GetImage(expected, testing);

            int nColumns = image.Width / numberWidth;
            int nRows = image.Height / numberWidth;

            int row = index / nColumns;
            int column = index % nColumns;

            if (row < nRows && column < nColumns)
            {
                return GetImageNumberPixels(expected, column, row, testing);
            }
            else
            {
                return null;
            }
        }
    }
}

