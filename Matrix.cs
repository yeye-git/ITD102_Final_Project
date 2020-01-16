﻿﻿using System;
using System.IO;
using System.Collections.Generic;



class Matrix
{
    #region Class Fields
    private double[,] _data;

    /// <summary>
    /// Number of rows in this matrix
    /// </summary>
    /// <value>number of rows</value>
    public int Row
    {
        get { return _data.GetLength(0); }
    }

    /// <summary>
    /// Number of columns in this matrix
    /// </summary>
    /// <value>number of columns</value>
    public int Column
    {
        get { return _data.GetLength(1); }
    }

    /// <summary>
    /// The transpose of the matrix
    /// </summary>
    /// <value>the transposed matrix</value>
    public Matrix T
    {
        // A = Row X Column
        // A_T = Column X Row 
        get
        {
            Matrix TransposedMatrix = new Matrix(this.Column, this.Row);
            for (int row = 0; row < this.Row; row++)
            {
                for (int col = 0; col < this.Column; col++)
                {
                    // change the position of row index and col index
                    TransposedMatrix[col, row] = this[row, col];
                }
            }
            return TransposedMatrix;
        }
    }

    /// <summary>
    /// return the shape of the matrix as a 1D int array[row,col] (for creating new matrix) 
    /// </summary>
    /// <value></value>
    public int[] Shape
    {
        get
        {
            int[] shape = { Row, Column };
            return shape;
        }
    }

    /// <summary>
    /// Return the size of the matrix as a string. e.g. "5 X 3"
    /// </summary>
    /// <value></value>
    public string Size
    {
        get { return $"{this.Row} X {this.Column}"; }
    }

    //[] overload
    /// <summary>
    /// get the element of the matrix using the given row and col index
    /// </summary>
    /// <value></value>
    public double this[int row, int col]
    {

        get
        {
            return _data[row, col];
        }
        set
        {
            _data[row, col] = value;
        }

    }

    /// <summary>
    /// only works for 1 column matrix, get the specific value
    /// </summary>
    /// <value></value>
    public double this[int row]
    {
        get
        {
            if (this.Column == 1)
            {
                return _data[row, 0];
            }
            else
            {
                throw new ArgumentException("Invalid indexing");
            }
        }
        set
        {
            if (this.Column == 1)
            {

                _data[row, 0] = value;
            }
            else
            {
                throw new ArgumentException("Invalid indexing");
            }
        }
    }

    /// <summary>
    /// Get the matrix as a double[,]
    /// </summary>
    /// <returns>a double[,] contains matrix values</returns>
    public double[,] GetData()
    {
        return this._data;
    }

    #endregion

    #region  Class Constructors
    /// <summary>
    /// construct an empty matrix with specific row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public Matrix(int row, int col)
    {
        _data = new double[row, col];
    }

    /// <summary>
    /// construct a matrix using 2D array
    /// </summary>
    /// <param name="input">a 2D array</param>
    public Matrix(double[,] input)
    {
        _data = input;
    }


    /// <summary>
    /// if input array has only 1 D,reshape it to two D array, e.g 784 => 28 * 28
    /// </summary>
    /// <param name="one_D_array">one dimension array</param>
    public Matrix(double[] one_D_array)
    {
        if (one_D_array.Length == 2)
        {
            double[,] two_D_version_ = new double[2, 1];
            two_D_version_[0, 0] = one_D_array[0];
            two_D_version_[1, 0] = one_D_array[1];
            _data = two_D_version_;
        }
        else
        {
            if (Math.Sqrt(one_D_array.Length) > (int)Math.Sqrt(one_D_array.Length))
            {
                throw new ArgumentException("The 1D array cannot be fully square rooted.");
            }

            // sqrt return a double
            // in order to reshape an array, the array has to be ** 1/2
            // not just /2 in length.
            double[,] two_D_version = new double[(int)(Math.Sqrt(one_D_array.Length)), (int)(Math.Sqrt(one_D_array.Length))];
            List<double> input_list = new List<double>();
            for (int x = 0; x < one_D_array.Length; x++)
            {
                input_list.Add(one_D_array[x]);
            }

            int index = 0;

            for (int row = 0; row < two_D_version.GetLength(0); row++)
            {
                for (int col = 0; col < two_D_version.GetLength(1); col++)
                {
                    try
                    {
                        two_D_version[row, col] = input_list[index];

                    }
                    catch
                    {
                        Console.WriteLine("out of range" + index);

                    }
                    index++;
                }
            }

            _data = two_D_version;
        }
    }

    /// <summary>
    /// Construct a matrix using a text file
    /// </summary>
    /// <param name="filePath">the file contains matrix</param>
    public Matrix(string filePath)
    {
        // get all the lines in the text file
        string[] lines = System.IO.File.ReadAllLines(filePath);

        // extract the matrix part and store in a list, line by line
        List<string> matrixList = new List<string>();

        // get the matrix part, and remove the {},
        foreach (string line in lines)
        {
            try// if empty break
            {
                // if it does not start with {, break the foreach loop
                if (line.Substring(0, 1) != "{")// if not { break
                {
                    break;
                }
            }// try is for empty line, if it is an empty line also break
            catch (ArgumentOutOfRangeException)
            {
                break;
            }
            matrixList.Add(line.Substring(1, line.Length - 3));//-3 means skip "},"
        }

        // get the number of the columns
        int numCols = matrixList[0].Split(",").Length;

        double[,] matrixArr = new double[matrixList.Count, numCols];
        int row = 0;

        foreach (string line in matrixList)
        {
            string[] numStr = line.Split(",");
            if (numStr.Length != numCols)
            {
                // e.g.
                // {1,2,3},
                // {4,5},
                throw new Exception($"The length of column should be the same,\n nColumn number: {numStr.Length}" +
                $" is not eaqual to the other row's column number {numCols}");
            }
            for (int col = 0; col < numStr.Length; col++)
            {
                try
                {
                    matrixArr[row, col] = double.Parse(numStr[col]);
                }
                catch (FormatException)
                {
                    // e.g. (1)
                    // {1,2,3},
                    // {4,5,d},
                    // e.g. (2)
                    // {1,2,3},
                    // {4,5,},
                    // e.g. (3)
                    // {12,3}  miss ,
                    throw new Exception("Matrix can only contain numbers");
                }
            }
            row++;
        }


        this._data = matrixArr;
    }

    /// <summary>
    /// Construct a matrix using an 1D int array, usually matrix.Shape
    /// </summary>
    /// <param name="shape">1D int array containing [row,col]</param>
    public Matrix(int[] shape)
    {
        if (shape.Length != 2) { throw new Exception("input must be a 1D int[] containing the shape"); }
        int row = shape[0];
        int col = shape[1];
        _data = new Matrix(row, col)._data;
    }

    /// <summary>
    /// Create a matrix with random numbers
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static Matrix RandomMatrix(int row, int col)
    {
        Random rng = new Random();
        Matrix new_matrix = new Matrix(row, col);
        for (int rowIndex = 0; rowIndex < new_matrix.Row; rowIndex++)
        {
            for (int colIndex = 0; colIndex < new_matrix.Column; colIndex++)
            {
                double randomNum = rng.NextDouble() * 100;
                new_matrix[rowIndex, colIndex] = randomNum;
            }
        }
        return new_matrix;
    }

    #endregion

    #region Operator Helpers

    /// <summary>
    /// matrix dot product
    /// </summary>
    /// <param name="rightMatrix"></param>
    /// <returns></returns>
    public Matrix Dot(Matrix rightMatrix)
    {
        Matrix output;

        // check whether row == col
        if (this.Column == rightMatrix.Row)//valid
        {
            output = new Matrix(row: this.Row, col: rightMatrix.Column);// output matrix initialize

            for (int row = 0; row < this.Row; row++)
            {
                for (int col = 0; col < rightMatrix.Column; col++)
                {
                    double cell = 0;
                    for (int leftColumnIndex = 0; leftColumnIndex < this.Column; leftColumnIndex++)
                    {
                        cell = cell + this[row, leftColumnIndex] * rightMatrix[leftColumnIndex, col];
                    }
                    output[row, col] = cell;
                }
            }
            return output;
        }
        else// if input is invalid
        {
            Console.WriteLine("left [column] and right [row] is not the same");
            Console.WriteLine($"{this.Column} != {rightMatrix.Row}");
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// element-wise addition
    /// </summary>
    /// <param name="rightMatrix"></param>
    /// <returns>new matrix</returns>
    public Matrix Add(Matrix rightMatrix)
    {
        Matrix result;
        // check the size
        if (this.Size != rightMatrix.Size)
        {
            Console.WriteLine("Cannot add these two matries together");
            throw new ArgumentException($"Orginal: The size of left matrix: {this.Size} is not equal to the right matrix: {rightMatrix.Size}");
        }
        else
        {
            result = new Matrix(this.Row, this.Column);

            for (int row = 0; row < result.Row; row++)
            {
                for (int col = 0; col < result.Column; col++)
                {
                    result[row, col] = this[row, col] + rightMatrix[row, col];
                }
            }
        }
        return result;
    }

    /// <summary>
    /// element-wise substraction
    /// </summary>
    /// <param name="rightMatrix"></param>
    /// <returns>new matrix</returns>
    public Matrix Substract(Matrix rightMatrix)
    {
        Matrix result;
        
        // check the size
        if (this.Size != rightMatrix.Size)
        {
            Console.WriteLine("Cannot substract these two matries");
            throw new ArgumentException($"Orginal: The size of left matrix: {this.Size} is not equal to the right matrix: {rightMatrix.Size}");
        }

        result = new Matrix(this.Row, this.Column);

        for (int row = 0; row < result.Row; row++)
        {
            for (int col = 0; col < result.Column; col++)
            {
                result[row, col] = this[row, col] - rightMatrix[row, col];
            }
        }

        return result;
    }

    /// <summary>
    /// element-wise multiplication
    /// </summary>
    /// <param name="num">the number to be multiplied</param>
    /// <returns>new matrix</returns>
    public Matrix Multiply(double num)
    {
        Matrix result = new Matrix(this.Shape);

        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                result[row, col] = this[row, col] * num;
            }
        }
        return result;
    }

    /// <summary>
    /// element-wise multiplication
    /// </summary>
    /// <param name="right"></param>
    /// <returns>Shape will remain unchanged</returns>
    public Matrix Multiply(Matrix right)
    {

        if (this.Size != right.Size)
        {
            throw new ArgumentException("Perform element wise multiplication between two matries,"
            + " their size has to be the same");
        }
        Matrix result = new Matrix(right.Shape);
        for (int row = 0; row < right.Row; row++)
        {
            for (int col = 0; col < right.Column; col++)
            {
                result[row, col] = this[row, col] * right[row, col];
            }
        }
        return result;
    }

    // for overloadding == and !=
    public bool Equals(Matrix other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        var other = (Matrix)obj;
        return this == other;
    }

    #endregion

    #region Operator Overloads

    /// <summary>
    /// Matrix dot product
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(Matrix left, Matrix right)
    {
        return left.Dot(right);
    }

    /// <summary>
    /// element-wise addition
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(Matrix left, Matrix right)
    {
        // for 1 x 1 matrix, expand the matrix
        if (right.Size == "1 X 1")
        {
            right = new Matrix(left.Shape).SetNum(right[0]);
        }
        Matrix result;
        result = left.Add(right);

        return result;
    }


    /// <summary>
    /// element-wise addition, make a matrix full of the number then add the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(Matrix left, double right)
    {
        Matrix right_matrix = new Matrix(left.Shape).SetNum(right);
        Matrix result = left + right_matrix;
        return result;
    }

    /// <summary>
    /// element-wise addition, make a matrix full of the number then add the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(double left, Matrix right)
    {
        // shape has to be the same as the right matrix
        Matrix left_matrix = new Matrix(right.Shape).SetNum(left);
        Matrix result = left_matrix + right;
        return result;
    }

    /// <summary>
    /// element-wise substraction, make a matrix full of the number then substract the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right number</param>
    /// <returns></returns>
    public static Matrix operator -(Matrix left, double right)
    {
        Matrix rightMatrix = new Matrix(left.Shape).SetNum(right);
        Matrix result = left - rightMatrix;
        return result;
    }

    /// <summary>
    /// element-wise substraction, make a matrix full of the number then substract the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator -(double left, Matrix right)
    {
        // shape has to be the same as the right matrix
        Matrix leftMatrix = new Matrix(right.Shape).SetNum(left);
        Matrix result = leftMatrix - right;
        return result;
    }

    /// <summary>
    /// element-wise substraction
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator -(Matrix left, Matrix right)
    {
        Matrix result;
        // boradcasting for 1 x 1 matrix
        // e.g. 5 x 5 matrix - 1 x 1 matrix
        // trun the 1 x 1 to 5 x 5 first
        if (right.Size == "1 X 1")
        {
            right = new Matrix(left.Shape).SetNum(right[0]);
        }
        result = left.Substract(right);

        return result;
    }

    /// <summary>
    /// element-wise multiplication
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(Matrix left, double right)
    {
        Matrix result;
        result = left.Multiply(right);
        return result;
    }

    /// <summary>
    /// element-wise multiplication
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(double left, Matrix right)
    {
        Matrix result;
        result = right.Multiply(left);
        return result;
    }

    /// <summary>
    /// element wise division
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator /(Matrix left, double right)
    {
        Matrix result = new Matrix(left.Shape);
        Matrix right_matrix = new Matrix(left.Shape).SetNum(right);

        for (int row = 0; row < left.Row; row++)
        {
            for (int col = 0; col < left.Column; col++)
            {
                result[row, col] = left[row, col] / right_matrix[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// element wise division
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator /(double left, Matrix right)
    {
        Matrix result = new Matrix(right.Shape);
        Matrix left_matrix = new Matrix(right.Shape).SetNum(left);

        for (int row = 0; row < right.Row; row++)
        {
            for (int col = 0; col < right.Column; col++)
            {
                result[row, col] = left_matrix[row, col] / right[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// element-wise division
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns></returns>
    public static Matrix operator /(Matrix left, Matrix right)
    {
        Matrix result = new Matrix(right.Shape);
        for (int row = 0; row < right.Row; row++)
        {
            for (int col = 0; col < right.Column; col++)
            {
                result[row, col] = left[row, col] / right[row, col];
            }
        }
        return result;
    }

    public override int GetHashCode()
    {
        // numCells * (row*col) * cellValue
        int numCells = this.Row * this.Column;

        double hashCode = 0;

        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                hashCode += numCells * (row * col) * this[row, col];
            }
        }


        return (int)hashCode;
    }

    /// <summary>
    /// Element-wise comparison of two matries
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns>true or false</returns>
    public static bool operator ==(Matrix left, Matrix right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (ReferenceEquals(left, null))
        {
            return false;
        }
        if (ReferenceEquals(right, null))
        {
            return false;
        }

        // check size
        if (left.Size != right.Size)
        {
            return false;
        }

        // check each value
        for (int row = 0; row < left.Row; row++)
        {
            for (int col = 0; col < left.Column; col++)
            {
                if (left[row, col] != right[row, col])
                {
                    return false;
                }
            }
        }

        // if all values are the same
        return true;
    }

    /// <summary>
    /// Element-wise comparison of two matries
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns>true or false</returns>
    public static bool operator !=(Matrix left, Matrix right)
    {
        return !(left == right);
    }

    #endregion

    #region Math Functions
    /// <summary>
    /// return the sum of the whole matrix as a 1 x 1 matrix
    /// </summary>
    /// <returns>1 X 1 Matrix</returns>
    public static Matrix Sum(Matrix matrix)
    {
        Matrix result = new Matrix(1, 1);
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                result[0, 0] = result[0, 0] + matrix[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// element wise, turn all number into absolute number. Size remain unchanged
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Matrix Abs(Matrix matrix)
    {
        Matrix result = new Matrix(matrix.Shape);
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                result[row, col] = Math.Abs(matrix[row, col]);
            }
        }
        return result;
    }

    /// <summary>
    /// element-wise power
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="num">the specific num to be raised to</param>
    /// <returns></returns>
    public static Matrix Power(Matrix matrix, double num)
    {
        Matrix result = new Matrix(matrix.Shape);
        for (int row = 0; row < result.Row; row++)
        {
            for (int col = 0; col < result.Column; col++)
            {
                result[row, col] = Math.Pow(matrix[row, col], num);
            }
        }
        return result;
    }

    /// <summary>
    /// Calculate the mean of the matrix
    /// </summary>
    /// <param name="matrix">the matrix to be calculated</param>
    /// <returns></returns>
    public static Matrix Mean(Matrix matrix)
    {
        double sum = 0;
        double n = matrix.Row * matrix.Column;
        double mean;
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                sum = sum + matrix[row, col];
            }
        }
        mean = sum / n;
        Matrix result = new Matrix(1, 1).SetNum(mean);

        return result;
    }

    /// <summary>
    /// element-wise Exp
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns>a matrix after calculation</returns>
    public static Matrix Exp(Matrix matrix)
    {
        Matrix newMatrix = new Matrix(matrix.Shape);

        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                newMatrix[row, col] = Math.Exp(matrix[row, col]);//e^num
            }
        }
        return newMatrix;
    }

    /// <summary>
    /// element-wise natural log (base e)
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Matrix Log(Matrix matrix)
    {

        Matrix newMatrix = new Matrix(matrix.Shape);

        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                newMatrix[row, col] = Math.Log(matrix[row, col]);
            }
        }
        return newMatrix;

    }

    /// <summary>
    /// element-wise tanh, (e^z - e^-z)/(e^z + e^-z)
    /// </summary>
    /// <param name="Z"></param>
    /// <returns>a matrix which has the same dimension as input</returns>
    public static Matrix tanh(Matrix Z)
    {
        Matrix result = new Matrix(Z.Shape);
        result = (Matrix.Exp(Z) - Matrix.Exp(-1 * Z)) / (Matrix.Exp(Z) + Matrix.Exp(-1 * Z));
        return result;
    }

    /// <summary>
    /// Use the kernel to 'slides' across the input matrix
    /// </summary>
    /// <param name="input">the input matrix</param>
    /// <param name="kernel">a squared kernel</param>
    /// <returns>the matrix after convolution</returns>
    public static Matrix Convolve(Matrix input, Matrix kernel)
    {
        // create a matrix with extra row and col
        int extraLength = kernel.Row / 2;
        int kernelLength = kernel.Row;

        Matrix extendedMatrix = new Matrix(extraLength + input.Row + extraLength,
                                           extraLength + input.Column + extraLength);

        // fill the extended matrix using the input matrix
        for (int row = 0; row < input.Row; row++)
        {
            for (int col = 0; col < input.Column; col++)
            {
                extendedMatrix[row + extraLength, col + extraLength] = input[row, col];
            }
        }

        // create the output matrix which has the same size as input matrix
        Matrix output = new Matrix(input.Shape);

        // fill the output matrix by sliding over the input matrix
        Matrix square = new Matrix(kernel.Shape);
        for (int row = 0; row < output.Row; row++)
        {
            for (int col = 0; col < output.Column; col++)
            {
                // fill the square using the extended matrix
                for (int i = 0; i < kernelLength; i++)
                {
                    for (int j = 0; j < kernelLength; j++)
                    {
                        square[i, j] = extendedMatrix[row + i, col + j];
                    }
                }
                output[row, col] = Matrix.Sum(square * kernel)[0];
            }
        }
        return output;
    }

    #endregion

    #region Matrix Non-Static Methods

    /// <summary>
    /// set all elements to a specific nummber
    /// </summary>
    /// <param name="num">the number to set</param>
    /// <returns>a matrix which is full of the number</returns>
    public Matrix SetNum(double num)
    {
        Matrix newMatrix = new Matrix(this.Shape);
        for (int row = 0; row < newMatrix.Row; row++)
        {
            for (int col = 0; col < newMatrix.Column; col++)
            {
                newMatrix[row, col] = num;
            }
        }
        return newMatrix;
    }

    /// <summary>
    /// reshape the matrix,supply row and column
    /// </summary>
    /// <param name="row">the number of rows</param>
    /// <param name="col">the number of columns</param>
    /// <returns>return a row x col matrix</returns>   
    public Matrix Reshape(int row, int col)
    {
        if (row * col != this.Row * this.Column)
        {
            Console.WriteLine("cannot reshpe this matrix");
            Console.WriteLine($"Orginal: {this.Row} X {this.Column} != Output: {row} X {col}");
            return this;
        }
        // create a list to store original matrix values (each cell)
        List<double> original_values = new List<double>();
        for (int original_row = 0; original_row < this.Row; original_row++)
        {
            for (int orginal_col = 0; orginal_col < this.Column; orginal_col++)
            {
                original_values.Add(this[original_row, orginal_col]);
            }
        }

        Matrix shapped_matrix;
        shapped_matrix = new Matrix(row, col);
        int list_index = 0;

        for (int shapped_row = 0; shapped_row < shapped_matrix.Row; shapped_row++)
        {
            for (int shapped_col = 0; shapped_col < shapped_matrix.Column; shapped_col++)
            {
                shapped_matrix[shapped_row, shapped_col] = original_values[list_index];
                list_index++;
            }
        }
        return shapped_matrix;
    }

    /// <summary>
    /// Reshape the matrix using column number only
    /// </summary>
    /// <param name="col">the number of columns</param>
    /// <returns>a reshaped matrix</returns>
    public Matrix Reshape(int col)
    {
        if (((this.Row * this.Column) % col) != 0)
        {
            // e.g. some reshpre might cause this
            // {1,2,3,4,5},
            // {6,7,8,9  } ---- miss one element
            throw new ArgumentException("The matrix cannot be perfectly reshaped");
        }
        int row = (this.Row * this.Column) / col;
        return this.Reshape(row, col);
    }

    /// <summary>
    /// Return a string conatining "Row X Column"
    /// </summary>
    /// <returns>"Row X Column"</returns>
    public override string ToString()
    {
        string result;

        result = $"Row X Column : {this.Row} X {this.Column}";

        return result;
    }

    /// <summary>
    /// Display the matrix
    /// </summary>
    /// <param name="numDecimals">the number of decimal spaces to use</param>
    public void Display(int numDecimals = 2)
    {
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                Console.Write(Math.Round(this[row, col], numDecimals) + "\t");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"\nThis is a {this.Row} x {this.Column} Matrix");
    }

    /// <summary>
    /// Convert the whole matrix into a string, that can be directly save as a text file
    /// </summary>
    /// <returns>a string containing the whole matrix</returns>
    public string ReturnString()
    {
        string text = "";
        for (int row = 0; row < this.Row; row++)
        {
            text = text + "{";
            for (int col = 0; col < this.Column; col++)
            {
                if (col == this.Column - 1)
                {
                    text = text + (this[row, col]);
                }
                else
                {
                    text = text + (this[row, col] + ",");
                }
            }
            text = text + "},\n";
        }
        return text;
    }

    /// <summary>
    /// turn the specific column into a new matrix (one column)
    /// </summary>
    /// <param name="colIndex">the specific column</param>
    /// <returns>one column matrix</returns>
    public Matrix GetColumn(int colIndex)
    {
        Matrix newMatrix = new Matrix(this.Row, 1);
        for (int row = 0; row < this.Row; row++)
        {
            newMatrix[row, 0] = this[row, colIndex];
        }
        return newMatrix;
    }

    /// <summary>
    /// turn the specific row into a new matrix (one row)
    /// </summary>
    /// <param name="rowIndex">the index of the row</param>
    /// <returns>one row matrix</returns>
    public Matrix GetRow(int rowIndex)
    {
        Matrix new_matrix = new Matrix(row: 1, col: this.Column);
        for (int col = 0; col < this.Column; col++)
        {
            new_matrix[0, col] = this[rowIndex, col];
        }
        return new_matrix;
    }

    /// <summary>
    /// Remove a specific column in the matrix
    /// </summary>
    /// <param name="colIndex">the column's index to be removed</param>
    /// <returns>return a new matrix after removing</returns>
    public Matrix RemoveColumn(int colIndex)
    {
        Matrix result;

        if (colIndex == 0)
        {
            result = new Matrix(this.Row, this.Column - 1);

            for (int row = 0; row < result.Row; row++)
            {
                for (int col = 0; col < result.Column; col++)
                {
                    result[row, col] = this[row, col + 1];
                }
            }
            return result;
        }
        // deal with col index != 0
        Matrix left_matrix = new Matrix(this.Row, colIndex);
        Matrix right_matrix = new Matrix(this.Row, this.Column - colIndex - 1);

        // populate the left matrix
        for (int row = 0; row < left_matrix.Row; row++)
        {
            for (int col = 0; col < left_matrix.Column; col++)
            {
                left_matrix[row, col] = this[row, col];
            }
        }

        // populate the right matrix
        for (int row = 0; row < right_matrix.Row; row++)
        {
            for (int col = 0; col < right_matrix.Column; col++)
            {
                right_matrix[row, col] = this[row, col + colIndex + 1];
            }
        }

        // combine left and right
        result = left_matrix.Concatenate(right_matrix);
        return result;
    }

    /// <summary>
    /// Remove a range of columns based on the number given
    /// </summary>
    /// <param name="colIndex">start index</param>
    /// <param name="numColToBeRemoved">number of columns to be removed</param>
    /// <returns></returns>
    public Matrix RemoveColumn(int colIndex, int numColToBeRemoved)
    {
        // new col  = orgiranl col - num_of_cols to be removed
        Matrix matrix = this;
        int numColRemoved = 0;
        while (true)
        {
            matrix = matrix.RemoveColumn(colIndex);
            numColRemoved++;
            if (numColRemoved == numColToBeRemoved) { break; }
        }
        return matrix;
    }

    /// <summary>
    /// Remove a specific row in the matrix
    /// </summary>
    /// <param name="rowIndex">the index of the row to be removed</param>
    /// <returns>return a new matrix after removing</returns>
    public Matrix RemoveRow(int rowIndex)
    {
        Matrix result;

        if (rowIndex == 0)
        {
            result = new Matrix(this.Row - 1, this.Column);

            // populate the result
            for (int row = 0; row < result.Row; row++)
            {
                for (int col = 0; col < result.Column; col++)
                {
                    result[row, col] = this[row + 1, col];
                }
            }
        }

        // deal with row_index != 0
        Matrix topMatrix = new Matrix(rowIndex, this.Column);
        Matrix bottomMatrix = new Matrix(this.Row - rowIndex - 1, this.Column);

        // populate the top matrix
        for (int row = 0; row < topMatrix.Row; row++)
        {
            for (int col = 0; col < topMatrix.Column; col++)
            {
                topMatrix[row, col] = this[row, col];
            }
        }

        // populate the right matrix
        for (int row = 0; row < bottomMatrix.Row; row++)
        {
            for (int col = 0; col < bottomMatrix.Column; col++)
            {
                bottomMatrix[row, col] = this[row + rowIndex + 1, col];
            }
        }

        // combine top and bootom
        result = topMatrix.BottomConcatenate(bottomMatrix);

        return result;

    }

    /// <summary>
    /// Remove a range of rows in the matrix
    /// </summary>
    /// <param name="rowIndex">start index</param>
    /// <param name="numRows">how many rows to be removed</param>
    /// <returns>return a new matrix after removing</returns>
    public Matrix RemoveRow(int rowIndex, int numRows)
    {
        // new row  = orgiranl row - num_of_rows to be removed
        Matrix new_matrix = this;
        int row_removed = 0;
        while (true)
        {
            new_matrix = new_matrix.RemoveRow(rowIndex);
            row_removed++;
            if (row_removed == numRows) { break; }
        }
        return new_matrix;
    }
    /// <summary>
    /// Concatenate two matries together,left to right
    /// </summary>
    /// <param name="right">the matrix to be combined</param>
    /// <returns>return the combined matrix, horizontally</returns>
    public Matrix Concatenate(Matrix right)
    {
        // check row number
        if (this.Row != right.Row)
        {
            throw new ArgumentException($"{this.Row}!={right.Row}\n row number has to be the same");
        }
        Matrix new_matrix = new Matrix(this.Row, this.Column + right.Column);
        // populate the new matrix by using the left matrix
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                new_matrix[row, col] = this[row, col];
            }
        }

        // using the right matrix
        for (int row = 0; row < right.Row; row++)
        {
            for (int col = this.Column; col < this.Column + right.Column; col++)
            {
                new_matrix[row, col] = right[row, col - this.Column];
            }
        }
        return new_matrix;

    }

    /// <summary>
    /// Concatenate the matrix together, top to bottom
    /// </summary>
    /// <param name="bottom">the matrix to be concatenated from bottom</param>
    /// <returns>a taller matrix, vertically</returns>
    public Matrix BottomConcatenate(Matrix bottom)
    {

        // check column number
        if (this.Column != bottom.Column)
        {
            throw new ArgumentException($"{this.Column}!={bottom.Column}\n both column number has to be the same");
        }

        // there will be some extra rows depends on the bottom matrix's row
        Matrix newMatrix = new Matrix(this.Row + bottom.Row, this.Column);

        // populate the new matrix by using the up (original) matrix
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                newMatrix[row, col] = this[row, col];
            }
        }

        // populate the extra part with the bottom matrix by iterating over the bottom matrix
        for (int row = 0; row < bottom.Row; row++)
        {
            for (int col = 0; col < bottom.Column; col++)
            {
                newMatrix[row + this.Row, col] = bottom[row, col];
            }
        }
        return newMatrix;
    }


    /// <summary>
    /// Save the matrix as a text file
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveMatrix(string filePath)
    {
        //string text_to_write = this.Return_String();
        //WriteToFile(text_to_write,file_path);
        using (StreamWriter sr = new StreamWriter(filePath))
        {
            for (int row = 0; row < this.Row; row++)
            {
                sr.Write("{");
                for (int col = 0; col < this.Column; col++)
                {
                    if (col == this.Column - 1)
                    {
                        sr.Write(this[row, col]);
                    }
                    else { sr.Write(this[row, col] + ","); }
                }
                sr.Write("},");
                sr.WriteLine();
            }
        }
    }


    #endregion

    #region Matrix Static Methods

    /// <summary>
    /// Get the max value of the column according to the given index
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="colIndex"></param>
    /// <returns>a matrix(1x1) which is the maximum value of the specific column</returns>
    public static Matrix GetMax(Matrix matrix, int colIndex)
    {
        double maxValue = double.MinValue;
        int maxIndex = 0;
        for (int row = 0; row < matrix.Row; row++)
        {
            if (matrix[row, colIndex] > maxValue)
            {
                maxValue = matrix[row, colIndex];
                maxIndex = row;
            }
        }
        Matrix result = new Matrix(1, 1).SetNum(maxValue);

        return result;
    }

    /// <summary>
    /// find the index with max score in each column and turn into 1 row matrix
    /// </summary>
    /// <param name="matrix">The matrix to be searched</param>
    /// <returns>Return an one row matrix</returns>
    public static Matrix GetMax(Matrix matrix)
    {
        const int NUM_ROWS_TO_RETURN = 1;

        // the number of column is the same as the input's column
        Matrix result = new Matrix(NUM_ROWS_TO_RETURN, matrix.Column);

        // fill the each column

        for (int col = 0; col < result.Column; col++)
        {
            result[0, col] = Matrix.GetMax(matrix, col)[0];
        }

        return result;
    }

    /// <summary>
    /// Convert the matrix into a 2D byte array
    /// </summary>
    /// <param name="matrix">the matrix to be converted</param>
    /// <returns>a 2D byte array</returns>
    public static byte[,] GetByteArray(Matrix matrix)
    {
        byte[,] result = new byte[matrix.Row, matrix.Column];
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                if (matrix[row, col] > 255)
                {
                    result[row, col] = 255;
                }
                else if (matrix[row, col] < 0)
                {
                    result[row, col] = 0;
                }
                else
                {
                    result[row, col] = (byte)matrix[row, col];
                }
            }
        }
        return result;
    }

    #endregion
}
