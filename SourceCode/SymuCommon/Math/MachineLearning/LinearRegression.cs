#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Common.Math.MachineLearning
{
    /// <summary>
    ///     Obsolete, should use ML.Net lib
    /// </summary>
    public static class LinearRegression
    {
        public static double[] Process(double[][] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Solve(Design(data));
        }

        private static double[][] Design(double[][] data)
        {
            // add a leading col of 1.0 values
            var rows = data.Length;
            var cols = data[0].Length;
            var result = MatrixCreate(rows, cols + 1);
            for (var i = 0; i < rows; ++i)
            {
                result[i][0] = 1.0;
            }

            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    result[i][j + 1] = data[i][j];
                }
            }

            return result;
        }

        private static double[] Solve(double[][] design)
        {
            // find linear regression coefficients
            // 1. peel off X matrix and Y vector
            var rows = design.Length;
            var cols = design[0].Length;
            var X = MatrixCreate(rows, cols - 1);
            var Y = MatrixCreate(rows, 1); // a column vector

            for (var i = 0; i < rows; ++i)
            {
                int j;
                for (j = 0; j < cols - 1; ++j)
                {
                    X[i][j] = design[i][j];
                }

                Y[i][0] = design[i][j]; // last column
            }

            // 2. B = inv(Xt * X) * Xt * y
            var Xt = MatrixTranspose(X);
            var XtX = MatrixProduct(Xt, X);
            var inv = MatrixInverse(XtX);
            var invXt = MatrixProduct(inv, Xt);

            var mResult = MatrixProduct(invXt, Y);
            var result = MatrixToVector(mResult);
            return result;
        } // Solve

        // ===== Matrix routines

        private static double[][] MatrixCreate(int rows, int cols)
        {
            // allocates/creates a matrix initialized to all 0.0
            // do error checking here
            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
            {
                result[i] = new double[cols];
            }

            return result;
        }

        private static double[] MatrixToVector(double[][] matrix)
        {
            // single column matrix to vector
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            if (cols != 1)
            {
                throw new Exception("Bad matrix");
            }

            var result = new double[rows];
            for (var i = 0; i < rows; ++i)
            {
                result[i] = matrix[i][0];
            }

            return result;
        }

        private static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            var aRows = matrixA.Length;
            var aCols = matrixA[0].Length;
            var bRows = matrixB.Length;
            var bCols = matrixB[0].Length;
            if (aCols != bRows)
            {
                throw new Exception("Non-conformable matrices in MatrixProduct");
            }

            var result = MatrixCreate(aRows, bCols);

            for (var i = 0; i < aRows; ++i) // each row of A
            {
                for (var j = 0; j < bCols; ++j) // each col of B
                {
                    for (var k = 0; k < aCols; ++k) // could use k < bRows
                    {
                        result[i][j] += matrixA[i][k] * matrixB[k][j];
                    }
                }
            }

            return result;
        }

        private static double[][] MatrixDecompose(double[][] matrix, out int[] perm,
            out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // returns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            if (rows != cols)
            {
                throw new Exception("Non-square mattrix");
            }

            var n = rows; // convenience

            var result = MatrixDuplicate(matrix); // 

            perm = new int[n]; // set up row permutation result
            for (var i = 0; i < n; ++i)
            {
                perm[i] = i;
            }

            toggle = 1; // toggle tracks row swaps

            for (var j = 0; j < n - 1; ++j) // each column
            {
                var colMax = System.Math.Abs(result[j][j]);
                var pRow = j;

                for (var i = j + 1; i < n; ++i) // reader Matt V needed this:
                {
                    if (System.Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = System.Math.Abs(result[i][j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    var rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    var tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // -------------------------------------------------------------
                // This part added later (not in original code) 
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row 
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    var goodRow = -1;
                    for (var row = j + 1; row < n; ++row)
                    {
                        if (result[row][j] != 0.0)
                        {
                            goodRow = row;
                        }
                    }

                    if (goodRow == -1)
                    {
                        throw new Exception("Cannot use Doolittle's method");
                    }

                    // swap rows so 0.0 no longer on diagonal
                    var rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    var tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
                // -------------------------------------------------------------

                //if (Math.Abs(result[j][j]) < 1.0E-20) // deprecated
                //  return null; // consider a throw

                for (var i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (var k = j + 1; k < n; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }
            } // main j column loop

            return result;
        } // MatrixDecompose

        private static double[][] MatrixInverse(double[][] matrix)
        {
            var n = matrix.Length;
            var result = MatrixDuplicate(matrix);
            var lum = MatrixDecompose(matrix,
                out var perm,
                out _);
            if (lum == null)
            {
                throw new Exception("Unable to compute inverse");
            }

            var b = new double[n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                    {
                        b[j] = 1.0;
                    }
                    else
                    {
                        b[j] = 0.0;
                    }
                }

                var x = HelperSolve(lum, b); // use decomposition

                for (var j = 0; j < n; ++j)
                {
                    result[j][i] = x[j];
                }
            }

            return result;
        }

        private static double[][] MatrixTranspose(double[][] matrix)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            var result = MatrixCreate(cols, rows); // note indexing
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    result[j][i] = matrix[i][j];
                }
            }

            return result;
        } // TransposeMatrix

        private static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            var n = luMatrix.Length;
            var x = new double[n];
            b.CopyTo(x, 0);

            for (var i = 1; i < n; ++i)
            {
                var sum = x[i];
                for (var j = 0; j < i; ++j)
                {
                    sum -= luMatrix[i][j] * x[j];
                }

                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (var i = n - 2; i >= 0; --i)
            {
                var sum = x[i];
                for (var j = i + 1; j < n; ++j)
                {
                    sum -= luMatrix[i][j] * x[j];
                }

                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        private static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix
            var result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (var i = 0; i < matrix.Length; ++i) // copy the values
            {
                for (var j = 0; j < matrix[i].Length; ++j)
                {
                    result[i][j] = matrix[i][j];
                }
            }

            return result;
        }
    }
} // ns