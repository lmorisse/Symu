#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

namespace Symu.Common.Math.ProbabilityDistributions
{
    /// <summary>
    ///     the discrete probability distribution of a random variable which takes the value 1 with probability p
    ///     and the value 0 with probability q=1-p
    ///     Less formally, it can be thought of as a model for the set of possible outcomes of any single experiment that asks
    ///     a yes–no question.
    ///     Such questions lead to outcomes that are boolean-valued: a single bit whose value is success/yes/true/one with
    ///     probability p and failure/no/false/zero with probability q.
    ///     It can be used to represent a (possibly biased) coin toss where 1 and 0 would represent "heads" and "tails" (or
    ///     vice versa)
    /// </summary>
    /// <remarks>Encapsulation of Math.Net.Bernoulli : https://numerics.mathdotnet.com/Probability.html</remarks>
    public static class Bernoulli
    {
        /// <summary>
        ///     Samples a Bernoulli distributed random variable.
        /// </summary>
        /// <param name="probability"> The probability (p) of generating true. Range: 0 ≤ p ≤ 1.</param>
        /// <returns>A sample from the Bernoulli distribution</returns>
        public static bool Sample(float probability)
        {
            return MathNet.Numerics.Distributions.Bernoulli.Sample(probability) == 1;
        }
    }
}