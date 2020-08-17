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
    ///     a log-normal (or lognormal) distribution is a continuous probability distribution of a random variable whose
    ///     logarithm is normally distributed
    ///     The parameter mu  is the mean or expectation of the distribution (and also its median and mode);
    ///     and sigma  is its standard deviation.
    ///     The variance of the distribution is sigma ^2.
    /// </summary>
    /// <remarks>Encapsulation of Math.Net.Bernoulli : https://numerics.mathdotnet.com/Probability.html</remarks>
    public static class LogNormal
    {
        /// <summary>
        ///     Generates a sample from the normal distribution using the Box-Muller algorithm.
        /// </summary>
        /// <param name="mean">The mean (μ) of the normal distribution.</param>
        /// <param name="stdDev">The standard deviation (σ) of the normal distribution. Range: σ ≥ 0.</param>
        /// <returns>a sample from the distribution.</returns>
        public static float Sample(float mean, float stdDev)
        {
            return System.Math.Abs(stdDev) < Constants.Tolerance
                ? mean
                : Convert.ToSingle(MathNet.Numerics.Distributions.LogNormal.Sample(mean, stdDev));
        }
    }
}