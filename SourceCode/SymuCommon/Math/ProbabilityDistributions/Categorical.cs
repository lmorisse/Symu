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
    ///     a categorical distribution (also called a generalized Bernoulli distribution, multinoulli distribution[1]) is a
    ///     discrete probability distribution
    ///     that describes the possible results of a random variable that can take on one of K possible categories,
    ///     with the probability of each category separately specified.
    ///     The parameters specifying the probabilities of each possible outcome are constrained only by the fact that each
    ///     must be in the range 0 to 1, and all must sum to 1.
    /// </summary>
    /// <example>distribution : 50% probability for category A, 30% for category B and 20% for category C </example>
    /// <remarks>Encapsulation of Math.Net.Categorical : https://numerics.mathdotnet.com/Probability.html</remarks>
    public static class Categorical
    {
        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static int SampleIndex(double[] probabilityMass)
        {
            return MathNet.Numerics.Distributions.Categorical.Sample(probabilityMass);
        }

        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass1">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass2">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass3">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static int SampleIndex(float probabilityMass1, float probabilityMass2, float probabilityMass3)
        {
            var probabilityMass = new double[]
            {
                probabilityMass1,
                probabilityMass2,
                probabilityMass3
            };
            return SampleIndex(probabilityMass);
        }

        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass1">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass2">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass3">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass4">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static int SampleIndex(float probabilityMass1, float probabilityMass2, float probabilityMass3,
            float probabilityMass4)
        {
            var probabilityMass = new double[]
            {
                probabilityMass1,
                probabilityMass2,
                probabilityMass3,
                probabilityMass4
            };
            return SampleIndex(probabilityMass);
        }

        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass1">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass2">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass3">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass4">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <param name="probabilityMass5">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static int SampleIndex(float probabilityMass1, float probabilityMass2, float probabilityMass3,
            float probabilityMass4, float probabilityMass5)
        {
            var probabilityMass = new double[]
            {
                probabilityMass1,
                probabilityMass2,
                probabilityMass3,
                probabilityMass4,
                probabilityMass5
            };
            return SampleIndex(probabilityMass);
        }

        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static double SampleValue(double[] probabilityMass)
        {
            if (probabilityMass is null)
            {
                throw new ArgumentNullException(nameof(probabilityMass));
            }

            var index = MathNet.Numerics.Distributions.Categorical.Sample(probabilityMass);
            return probabilityMass[index];
        }

        /// <summary>
        ///     Samples one categorical distributed random variable
        /// </summary>
        /// <param name="probabilityMass">An array of nonnegative ratios. Not assumed to be normalized.</param>
        /// <returns>
        ///     One random integer between 0 and the size of the categorical (exclusive) representing the index of
        ///     probabilityMass
        /// </returns>
        public static float SampleValue(float[] probabilityMass)
        {
            if (probabilityMass is null)
            {
                throw new ArgumentNullException(nameof(probabilityMass));
            }

            var probabilityMassDouble = Array.ConvertAll(probabilityMass, x => (double) x);
            var index = MathNet.Numerics.Distributions.Categorical.Sample(probabilityMassDouble);
            return probabilityMass[index];
        }
    }
}