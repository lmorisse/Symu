#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Common.Math.ProbabilityDistributions
{
    /// <summary>
    ///     the discrete uniform distribution is a symmetric probability distribution wherein a finite number of values are
    ///     equally likely to be observed;
    ///     every one of n values has equal probability 1/n.
    /// </summary>
    /// <example>distribution : 50% probability for category A, 30% for category B and 20% for category C </example>
    /// <remarks>Encapsulation of Math.Net.Categorical</remarks>
    public static class DiscreteUniform
    {
        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [lower, upper]
        /// </summary>
        /// <param name="lower"> Lower bound, inclusive. Range: lower ≤ upper</param>
        /// <param name="upper">Upper bound, inclusive. Range: lower ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution [lower, upper]</returns>
        public static int Sample(int lower, int upper)
        {
            return MathNet.Numerics.Distributions.DiscreteUniform.Sample(lower, upper);
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [lower, upper]
        /// </summary>
        /// <param name="lower"> Lower bound, inclusive. Range: lower ≤ upper</param>
        /// <param name="upper">Upper bound, inclusive. Range: lower ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution [lower, upper]</returns>
        public static byte SampleToByte(byte lower, byte upper)
        {
            return Convert.ToByte(MathNet.Numerics.Distributions.DiscreteUniform.Sample(lower, upper));
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [mean-standardDeviation, mean+standardDeviation]
        /// </summary>
        /// <param name="mean"> </param>
        /// <param name="standardDeviation"></param>
        /// <returns>A sample from the discrete uniform distribution [mean-standardDeviation, mean+standardDeviation]</returns>
        public static byte SampleByMeanToByte(byte mean, byte standardDeviation)
        {
            byte lower = 0;
            if (mean >= standardDeviation)
            {
                lower = (byte) (mean - standardDeviation);
            }

            var upper = (byte) (mean + standardDeviation);
            return SampleToByte(lower, upper);
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [0, upper]
        /// </summary>
        /// <param name="upper">Upper bound, inclusive. Range: 0 ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution</returns>
        public static int Sample(int upper)
        {
            return MathNet.Numerics.Distributions.DiscreteUniform.Sample(0, upper);
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [0, upper]
        /// </summary>
        /// <param name="upper">Upper bound, inclusive. Range: 0 ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution</returns>
        public static byte SampleToByte(int upper)
        {
            return Convert.ToByte(Sample(upper));
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [lower, upper]
        /// </summary>
        /// <param name="length"></param>
        /// <param name="lower"> Lower bound, inclusive. Range: lower ≤ upper</param>
        /// <param name="upper">Upper bound, inclusive. Range: lower ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution [lower, upper]</returns>
        public static float[] Samples(int length, int lower, int upper)
        {
            var values = new int[length];
            MathNet.Numerics.Distributions.DiscreteUniform.Samples(values, lower, upper);
            return Array.ConvertAll(values, x => (float) x);
        }

        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [0, upper]
        /// </summary>
        /// <param name="length"></param>
        /// <param name="upper">Upper bound, inclusive. Range: 0 ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution [lower, upper]</returns>
        public static byte[] SamplesToByte(int length, int upper)
        {
            var values = new int[length];
            MathNet.Numerics.Distributions.DiscreteUniform.Samples(values, 0, upper);
            return Array.ConvertAll(values, x => (byte) x);
        }
    }
}