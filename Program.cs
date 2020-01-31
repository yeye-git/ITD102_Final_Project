using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Final_Project
{
    // pull test
    class Program
    {
        static void Main(string[] args)
        {
            string USAGE_INFO = @"
Usage:
Train the model
dotnet run -mode train -trainFolder <path> -labelWord <string> -numIterations <int> -learningRate <double>
    Parameters:
        -trainFolder <path> : the path of the directory that contains training images
        -labelWord <string> : if an image's file name contains the labelWord, it would be identified as a correct answer
        -numIterations <int> : the number of iterations to train the model (grandient descend)
        -learningRate <double> : the learning rate to train the model

Test the model
dotnet run -mode test -input <path>
    Parameters:
        -input <path> the path to the input image";
            string mode = GetArgValue(args,"-mode");
            // train command line
            if(mode=="train")
            {
                string trainFolder = GetArgValue(args, "-trainFolder");
                string labelWord = GetArgValue(args, "-labelWord");
                int numIterations = int.Parse(GetArgValue(args, "-numIterations"));
                double learningRate = double.Parse(GetArgValue(args,"-learningRate"));
                // This method will generate w.data and b.data
                Train(trainFolder,labelWord,numIterations,learningRate);
            }
            else if (mode=="test")
            {
                string inputPath = GetArgValue(args, "-input");
                Image test = new Image(inputPath);
                // Loads weight and bias created by the train method
                Matrix w = new Matrix("w.data");
                Matrix b = new Matrix("b.data");

                Test(test,w,b);
            }
            else
            {
                Console.WriteLine(USAGE_INFO);
                return;
            }
        }

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

        public static Matrix Cost(Matrix A, Matrix Y)
        {
            double m = Y.Column;
            Matrix loss = -1 * (Matrix.Multiply(Y, Matrix.Log(A)) + Matrix.Multiply((1 - Y), Matrix.Log(1 - A)));
            Matrix cost = (1 / m) * (Matrix.Sum(loss));

            return cost;
        }
        // Returns x
        // x.Shape(nx,1)
        static Matrix ImageToMatrix(Image image)
        {
            Matrix result = new Matrix(image.Height * image.Width * 3, 1);

            List<byte> RGB = new List<byte>();

            byte[] rgbArr = new byte[image.Height * image.Width * 3];


            //======= O(4n)
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
            // =======

            // stretch pixels into single column
            for (int i = 0; i < result.Row; i++)
            {
                result[i, 0] = RGB[i];
            }

            return result;
        }

        // Returns Matrix[] input = [X, Y]
        // X.Shape = [nx,m]
        // Y.Shape = [1,m]
        static Matrix[] LoadInput(string inputFolder, string labelWord, int width = 500, int height = 500)
        {
            Console.WriteLine("Loading the images....");
            Image input;

            string[] imagePaths = Directory.GetFiles(inputFolder);
            int m = imagePaths.GetLength(0);
            int nx = width * height * 3;

            Matrix X = new Matrix(nx, 1);
            Matrix Y = new Matrix(1, m);

            Matrix x;
            for (int i = 0; i < imagePaths.GetLength(0); i++)
            {
                // load every image and then resize the image according to the method paramters
                input = new Image(imagePaths[i]);
                input = Image.Resize(input, width, height);

                // convert to matrix
                x = ImageToMatrix(input);
                X = X.Concatenate(x);

                if (imagePaths[i].Contains(labelWord))
                {
                    Y[0, i] = 1;
                }
            }
            X = X.RemoveColumn(0);

            return new Matrix[] { X, Y };
        }

        static void Train(string trainFolder,string labelWord, int numIterations = 10, double learningRate = 0.003)
        {
            Matrix[] input = LoadInput(trainFolder, labelWord, 500, 500);

            Matrix X = input[0];
            Matrix Y = input[1];
            Matrix w = new Matrix(X.Row, 1);
            Matrix b = new Matrix(1, 1).SetNum(3); // a raw number
            double m = X.Column; // number of training examples


            for (int i = 0; i < numIterations; i++)
            {
                // forward propagation
                Matrix Z = w.T * X + b;
                Matrix A = Sigmoid(Z);
                Matrix cost = Cost(A, Y);
                cost.Display();

                // backward propagation
                Matrix dZ = A - Y;
                Matrix dW = (1 / m) * X * dZ.T;
                Matrix db = (1 / m) * Matrix.Sum(dZ);

                // gradient descend
                w = w - learningRate * dW;
                b = b - learningRate * db;

                // display message
                Z.Display();
                A.Display();
                Console.WriteLine($"Iteration: {i}\n");
            }
            w.SaveMatrix("w.data");
            b.SaveMatrix("b.data");
        }

        static void Test(Image test, Matrix w, Matrix b)
        {
            // load an input image and resize it
            test = Image.Resize(test, 500, 500);

            // converts the input image to a matrix
            Matrix x = ImageToMatrix(test);

            // predict
            Matrix z = w.T * x + b;
            Matrix a = Sigmoid(z); // output
            Console.WriteLine(a[0]);
        }
        /// <summary>
        /// Get the value of the corresponding agrument
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="argToFind">the argument to find</param>
        /// <returns></returns>
        public static string GetArgValue(string[] args, string argToFind)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == argToFind)
                {
                    try
                    {
                        return args[i + 1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return "";
                    }
                }
            }
            // if not exists
            return "";
        }


        /// <summary>
        /// returns true or false depending on whether args contains
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="argToFind">the specific arg to find</param>
        /// <returns></returns>
        public static bool CheckSingularArg(string[] args, string argToFind)
        {
            foreach (string arg in args)
            {
                if (arg == argToFind)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
