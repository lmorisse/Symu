#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Networks.Beliefs
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

        public Belief(ushort beliefId, string name, byte length, RandomGenerator model,
            BeliefWeightLevel beliefWeightLevel)
        {
            Id = beliefId;
            Length = length;
            Name = name;
            InitializeWeights(model, length, beliefWeightLevel);
        }

        /// <summary>
        ///     Belief Id
        /// </summary>
        public ushort Id { get; }

        public string Name { get; }

        /// <summary>
        ///     Each area of belief is represented by a collection of BeliefBits
        ///     The size define the length of the collection
        ///     each bit represent a single atomic belief
        ///     size range [0; 10]
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
        ///     Transform BeliefLevel into a value between [0;1]
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static float GetValueFromBeliefLevel(BeliefLevel level)
        {
            return ContinuousUniform.Sample(GetMinFromBeliefLevel(level), GetMaxFromBeliefLevel(level));
        }

        public static float GetMaxFromBeliefLevel(BeliefLevel level)
        {
            switch (level)
            {
                case BeliefLevel.NoBelief:
                    return 0;
                case BeliefLevel.StronglyDisagree:
                    return -0.75F;
                case BeliefLevel.Disagree:
                    return -0.25F;
                case BeliefLevel.NeitherAgreeNorDisagree:
                    return 0.25F;
                case BeliefLevel.Agree:
                    return 0.75F;
                case BeliefLevel.StronglyAgree:
                    return 1F;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public static float GetMinFromBeliefLevel(BeliefLevel level)
        {
            switch (level)
            {
                case BeliefLevel.NoBelief:
                    return 0;
                case BeliefLevel.StronglyDisagree:
                    return -1F;
                case BeliefLevel.Disagree:
                    return -0.75F;
                case BeliefLevel.NeitherAgreeNorDisagree:
                    return -0.25F;
                case BeliefLevel.Agree:
                    return 0.25F;
                case BeliefLevel.StronglyAgree:
                    return 0.75F;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        ///     Given a random model
        ///     set the weights : an array fill of random float ranging [-1; 1]
        ///     representing the detailed Belief of an agent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="length"></param>
        /// <param name="beliefWeightLevel"></param>
        /// <returns></returns>
        public void InitializeWeights(RandomGenerator model, byte length, BeliefWeightLevel beliefWeightLevel)
        {
            float[] beliefBits;
            switch (beliefWeightLevel)
            {
                case BeliefWeightLevel.NoWeight:
                    beliefBits = DiscreteUniform.Samples(length, 0, 0);
                    break;
                case BeliefWeightLevel.RandomWeight:
                    beliefBits = model == RandomGenerator.RandomUniform
                        ? ContinuousUniform.Samples(length, 0, RangeMax)
                        : DiscreteUniform.Samples(length, 0, RangeMax);
                    break;
                case BeliefWeightLevel.FullWeight:
                    beliefBits = DiscreteUniform.Samples(length, 1, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(beliefWeightLevel), beliefWeightLevel, null);
            }

            Weights = new Bits(beliefBits, 0);
        }

        /// <summary>
        ///     Given a random model and a BeliefLevel
        ///     return the beliefBits for the agent: an array fill of random binaries
        ///     representing the detailed belief of an agent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="beliefLevel"></param>
        /// <returns></returns>
        public float[] InitializeBits(RandomGenerator model, BeliefLevel beliefLevel)
        {
            float[] beliefBits;
            switch (model)
            {
                case RandomGenerator.RandomUniform:
                {
                    float min;
                    float max;

                    switch (beliefLevel)
                    {
                        case BeliefLevel.Random:
                            min = RangeMin;
                            max = RangeMax;
                            break;
                        default:
                            min = GetMinFromBeliefLevel(beliefLevel);
                            max = GetMaxFromBeliefLevel(beliefLevel);
                            break;
                    }

                    beliefBits = ContinuousUniform.Samples(Length, min, max);
                    break;
                }
                case RandomGenerator.RandomBinary:
                {
                    switch (beliefLevel)
                    {
                        case BeliefLevel.Random:
                            beliefBits = ContinuousUniform.FilteredSamples(Length, RangeMin, RangeMax);
                            break;
                        default:
                            var mean = 1 - GetValueFromBeliefLevel(beliefLevel);
                            beliefBits = ContinuousUniform.FilteredSamples(Length, mean);
                            break;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }

            return beliefBits;
        }
    }
}