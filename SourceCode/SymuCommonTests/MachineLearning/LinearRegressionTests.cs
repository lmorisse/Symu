#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Math.MachineLearning;

#endregion

namespace SymuToolsTests.MachineLearning
{
    [TestClass]
    public class LinearRegressionTest
    {
        [TestMethod]
        public void LinearRegressionWith1CoefTest()
        {
            var rows = 10;
            var data = new double[rows][];
            for (var i = 0; i < rows; ++i)
            {
                data[i] = new double[2];
            }

            for (var i = 0; i < rows; ++i)
            {
                // y = coef1*x + coef0
                data[i][0] = i; //x
                data[i][1] = i; //y
            }

            var coef = LinearRegression.Process(data); // use design matrix

            Assert.AreEqual(30, System.Math.Round(coef[1] * 30 + coef[0]));
        } // Main

        [TestMethod]
        public void RandomLinearRegressionWith1CoefTest()
        {
            var rows = 15;
            var data = new double[rows][];
            for (var i = 0; i < rows; ++i)
            {
                data[i] = new double[2];
            }

            for (var i = 0; i < rows; ++i)
            {
                // y = coef0 + coef1*x
                data[i][0] = i; //x
            }

            data[0][1] = 1;
            data[1][1] = 1;
            data[2][1] = 2;
            data[3][1] = 1;
            data[4][1] = 3;
            data[5][1] = 1;
            data[6][1] = 2;
            data[7][1] = 1;
            data[8][1] = 2;
            data[9][1] = 3;
            data[10][1] = 4;
            data[11][1] = 2;
            data[12][1] = 3;
            data[13][1] = 2;
            data[14][1] = 4;
            var coef = LinearRegression.Process(data); // use design matrix

            Assert.AreEqual(3.52, System.Math.Round(coef[1] * 16 + coef[0], 2));
            Assert.AreEqual(5.67, System.Math.Round(coef[1] * 30 + coef[0], 2));
        } // Main


        [TestMethod]
        public void LinearRegressionDummyTest()
        {
            var rows = 10;
            var seed = 1;

            var data = DummyData(rows, seed);

            var coef = LinearRegression.Process(data); // use design matrix

            var y = Income(14, 12, 0, coef);
            Assert.AreEqual(33, System.Math.Round(y));
        } // Main

        private static double[][] DummyData(int rows, int seed)
        {
            // generate dummy data for linear regression problem
            var b0 = 15.0;
            var b1 = 0.8; // education years
            var b2 = 0.5; // work years
            var b3 = -3.0; // sex = 0 male, 1 female
            var rnd = new Random(seed);

            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
            {
                result[i] = new double[4];
            }

            for (var i = 0; i < rows; ++i)
            {
                var ed = rnd.Next(12, 17); // 12, 16]
                var work = rnd.Next(10, 31); // [10, 30]
                var sex = rnd.Next(0, 2); // 0 or 1
                var y = b0 + b1 * ed + b2 * work + b3 * sex;
                y += 10.0 * rnd.NextDouble() - 5.0; // random [-5 +5]

                result[i][0] = ed;
                result[i][1] = work;
                result[i][2] = sex;
                result[i][3] = y; // income
            }

            return result;
        }

        private static double Income(double x1, double x2, double x3, double[] coef)
        {
            // x1 = education, x2 = work, x3 = sex
            double result; // the constant
            result = coef[0] + x1 * coef[1] + x2 * coef[2] + x3 * coef[3];
            return result;
        }
    } // Program
} // ns