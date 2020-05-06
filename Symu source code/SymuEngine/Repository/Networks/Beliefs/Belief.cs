#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.ProbabilityDistributions;

#endregion

namespace SymuEngine.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Describe a belief, based on knowledge/fact
    /// </summary>
    public class Belief
    {
        /// <summary>
        ///     Range min = disagreement
        /// </summary>
        private const int RangeMin = -1;

        /// <summary>
        ///     Range min = agreement
        /// </summary>
        private const int RangeMax = 1;

        public Belief(ushort beliefId, byte length, RandomGenerator model)
        {
            Id = beliefId;
            Length = length;
            InitializeWeights(model, length);
        }

        /// <summary>
        ///     Belief Id
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        ///     Length
        /// </summary>
        public byte Length { get; }

        /// <summary>
        ///     BeliefWeights represented by a collection of Bits ranging [-1;1]
        ///     give he impact of a Bit on a belief
        /// </summary>
        public Bits Weights { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is Belief belief
                   && Id == belief.Id;
        }

        /// <summary>
        ///     Given a KnowledgeModel
        ///     set the weights : an array fill of random float ranging [-1; 1]
        ///     representing the detailed Belief of an agent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public void InitializeWeights(RandomGenerator model, byte length)
        {
            var beliefBits = model == RandomGenerator.RandomUniform
                ? ContinuousUniform.Samples(length, RangeMin, RangeMax)
                : DiscreteUniform.Samples(length, RangeMin, RangeMax);

            Weights = new Bits(beliefBits, RangeMin);
        }
    }
}