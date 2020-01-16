using System;
using System.IO;
using System.Collections.Generic;

namespace Final_Project
{

    class Program
    {

        /// <summary>
        /// Sigmoid function, take Z, return number between 0 - 1
        /// </summary>
        /// <param name="Z">W.T * X + b</param>
        /// <returns>A, size is the same as input</returns>
        public static Matrix Sigmoid(Matrix Z)
        {
            Matrix A = new Matrix(Z.Shape);
            A = 1 / (1 + Matrix.Exp(-1 * Z));
            return A;
        }
        // Return x
        // x.Shape(nx,1)
        static Matrix ConvertImageToMatrix(Image image)
        {
            Matrix result = new Matrix(image.Height * image.Width * 3, 1);
            List<byte> RGB = new List<byte>();

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    RGB.Add(image[i, j].R);
                }
            }

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    RGB.Add(image[i, j].G);
                }
            }

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    RGB.Add(image[i, j].B);
                }
            }

            for (int i = 0; i < result.Row; i++)
            {
                result[i, 0] = RGB[i];
            }

            return result;
        }

        // Returns Matrix[] input = [X, Y]
        // X.Shape = [nx,m]
        // Y.Shape = [1,m]
        static Matrix[] LoadInput(string inputFolder)
        {
            Image input;
            string labelWord = "yeye";
            int width = 1280;
            int height = 720;
            // image size should be 1280 x 720
            string[] imagePaths = Directory.GetFiles(inputFolder);
            int m = imagePaths.GetLength(0);
            int nx = width * height * 3;

            Matrix X = new Matrix(nx, 1);
            Matrix Y = new Matrix(1, m);

            Matrix x;
            for (int i = 0; i < imagePaths.GetLength(0); i++)
            {
                input = new Image(imagePaths[i]);
                input = Image.Resize(input, width, height);
                x = ConvertImageToMatrix(input);
                X = X.Concatenate(x);

                if (imagePaths[i].Contains(labelWord))
                {
                    Y[0, i] = 1;
                }
            }
            X = X.RemoveColumn(0);

            return new Matrix[] { X, Y };
        }

    
        static void Train()
        {
            Matrix[] input = LoadInput("trainFolder");

            Matrix X = input[0];
            Matrix Y = input[1];
            Matrix W = new Matrix(X.Row, 1);
            Matrix b = new Matrix(1, 1).SetNum(3);
            int numIterations = 100;
            for(int i = 0; i <numIterations;i++)
            {
                Matrix Z = W.T * X + b;
                Matrix A = Sigmoid(Z);

                Z.Display();
                A.Display();
                Console.WriteLine($"Iteration: {i}\n");

                double m = X.Column;
                Matrix dZ = A - Y;
                Matrix dW = (1 / m) * X * dZ.T;
                Matrix db = (1 / m) * Matrix.Sum(dZ);

                double learningRate = 0.3;
                W = W - learningRate * dW;
                b = b - learningRate * db;
            }

        }
        static void Main()
        {
            Image test = new Image("testFolder/test4.jpg");
            test = Image.Resize(test,1280,720);
            
            
            
            
            Matrix x = ConvertImageToMatrix(test);
            Matrix W = new Matrix("W.txt");
            Matrix b = new Matrix("b.txt");

            Matrix z = W.T* x + b;
            Matrix a = Sigmoid(z);
            a.Display();
        }
    }
}
