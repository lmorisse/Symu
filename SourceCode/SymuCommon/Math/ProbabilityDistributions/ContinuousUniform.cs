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
    ///     The distribution describes an experiment where there is an arbitrary outcome that lies between certain bounds.
    ///     The bounds are defined by the parameters, a and b, which are the minimum and maximum values.
    /// </summary>
    /// <remarks>Encapsulation of Math.Net.Categorical</remarks>
    public static class ContinuousUniform
    {
        /// <summary>
        ///     Samples a uniformly distributed random variable in the range [lower, upper]
        /// </summary>
        /// <param name="lower"> Lower bound, inclusive. Range: lower ≤ upper</param>
        /// <param name="upper">Upper bound, inclusive. Range: lower ≤ upper</param>
        /// <returns>A sample from the discrete uniform distribution [lower, upper]</returns>
        public static float Sample(float lower, float upper)
        {
            return (float) MathNet.Numerics.Distributions.ContinuousUniform.Sample(lower, upper);
        }

        /// <summary>
        ///     Fills an array with samples generated from the distribution.
        /// </summary>
        /// <param name="length">the length of the array to fill</param>
        /// <param name="lower"> Lower bound. Range: lower ≤ upper.</param>
        /// <param name="upper">Upper bound. Range: lower ≤ upper.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public static float[] Samples(int length, float lower, float upper)
        {
            var values = new double[length];
            MathNet.Numerics.Distributions.ContinuousUniform.Samples(values, lower, upper);
            return Array.ConvertAll(values, x => (float) x);
        }

        /// <summary>
        ///     Fill randomly an binary array
        ///     with 0 and 1 with a uniform distribution.
        /// </summary>
        /// <param name="length">Dimension of the array</param>
        /// <param name="threshold">
        ///     Value below the mean is converted in 0
        ///     Value above the mean is converted in 1
        ///     Ranging [0;1]
        /// </param>
        /// <returns></returns>
        public static float[] FilteredSamples(byte length, float threshold)
        {
            return FilteredSamples(length, threshold, 1);
        }

        /// <summary>
        ///     Fill randomly an binary array
        ///     with 0 and Value with a uniform distribution.
        /// </summary>
        /// <param name="length">Dimension of the array</param>
        /// <param name="threshold">
        ///     Value below the threshold is set to 0
        ///     Value above the threshold is set to value
        ///     Ranging [0;1]
        /// </param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float[] FilteredSamples(byte length, float threshold, float value)
        {
            var randomNumber = Samples(length, 0, 1);
            var binaryArray = new float[length];
            for (byte i = 0; i < length; i++)
            {
                binaryArray[i] = randomNumber[i] < threshold ? 0 : value;
            }

            return binaryArray;
        }
    }
}