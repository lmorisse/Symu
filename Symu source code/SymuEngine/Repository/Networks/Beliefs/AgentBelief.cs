#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Classes.ProbabilityDistributions;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Describe the Knowledge of an agent :
    ///     KnowledgeId, KnowledgeLevel, KnowledgeBits
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class AgentBelief
    {
        /// <summary>
        ///     Range min = disagreement
        /// </summary>
        private const int RangeMin = -1;

        /// <summary>
        ///     Range min = agreement
        /// </summary>
        private const int RangeMax = 1;

        public AgentBelief(ushort beliefId)
        {
            BeliefId = beliefId;
        }

        public ushort BeliefId { get; }
        public Bits BeliefBits { get; set; }

        public byte Length => BeliefBits?.Length ?? 0;

        /// <summary>
        ///     Given a KnowledgeModel
        ///     set the weights : an array fill of random float ranging [-1; 1]
        ///     representing the detailed Belief of an agent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="length"></param>
        /// <param name="neutral">If set, initialize agent belief to neutral value (O)</param>
        /// <returns></returns>
        public void InitializeBeliefBits(RandomGenerator model, byte length, bool neutral)
        {
            float[] beliefBits;
            if (neutral)
            {
                beliefBits = new float[length];
                for (var i = 0; i < length; i++)
                {
                    beliefBits.SetValue(0, i);
                }
            }
            else
            {
                switch (model)
                {
                    case RandomGenerator.RandomUniform:
                        beliefBits = ContinuousUniform.Samples(length, RangeMin, RangeMax);
                        break;
                    case RandomGenerator.RandomBinary:
                        beliefBits = DiscreteUniform.Samples(length, RangeMin, RangeMax);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(model), model, null);
                }
            }

            BeliefBits = new Bits(beliefBits, RangeMin);
        }

        /// <summary>
        ///     Check the agent beliefs against the taskKnowledges
        /// </summary>
        /// <param name="taskKnowledgeIndexes"></param>
        /// <param name="index"></param>
        /// <param name="belief"></param>
        /// <param name="beliefThreshHoldForReacting"></param>
        /// <returns>The normalized score of the agent belief [-1; 1]</returns>
        public float Check(byte[] taskKnowledgeIndexes, out byte index, Belief belief,
            float beliefThreshHoldForReacting)
        {
            if (taskKnowledgeIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskKnowledgeIndexes));
            }

            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            if (BeliefBits is null)
            {
                throw new ArgumentNullException(nameof(BeliefBits));
            }

            index = 0;
            if (taskKnowledgeIndexes.Length == 0)
            {
                return 0;
            }

            for (byte i = 0; i < taskKnowledgeIndexes.Length; i++)
            {
                index = taskKnowledgeIndexes[i];
                var score = belief.Weights.GetBit(index) * BeliefBits.GetBit(index);

                if (Math.Abs(score) >= beliefThreshHoldForReacting)
                {
                    return score;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits filtered by minimumKnowledge
        ///     if a KnowledgeBit inferior minimumKnowledge then KnowledgeBit = 0
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits CloneWrittenBeliefBits(float minimumBelief)
        {
            var clone = BeliefBits.Clone();

            if (clone.IsNull)
            {
                return null;
            }

            for (byte i = 0; i < clone.Length; i++)
                // Intentionally strictly < 
                // Belief may be < 0, so it's the absolute value of belief that we check
            {
                if (Math.Abs(clone.GetBit(i)) < minimumBelief)
                {
                    clone.SetBit(i, 0);
                }
            }

            return clone;
        }

        public bool BelievesEnough(byte index, float beliefThreshHoldForAnswer)
        {
            if (Length == 0)
            {
                return false;
            }

            if (index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            // BeliefBits range is [-1;1], so absolute value is required
            return Math.Abs(BeliefBits.GetBit(index)) >= beliefThreshHoldForAnswer;
        }

        /// <summary>
        ///     Learn from beliefBits with a weight of influenceWeight
        /// </summary>
        /// <param name="beliefBits"></param>
        /// <param name="influenceWeight">range [-1; 1] based on influenceability and influentialness</param>
        public void Learn(Bits beliefBits, float influenceWeight)
        {
            if (beliefBits is null)
            {
                throw new ArgumentNullException(nameof(beliefBits));
            }

            for (byte i = 0; i < beliefBits.Length; i++)
            {
                if (Math.Abs(beliefBits.GetBit(i)) < Tolerance)
                {
                    continue;
                }

                var learnedBit = BeliefBits.GetBit(i) + beliefBits.GetBit(i) * influenceWeight;
                if (learnedBit > RangeMax)
                {
                    learnedBit = RangeMax;
                }

                if (learnedBit < RangeMin)
                {
                    learnedBit = RangeMin;
                }

                BeliefBits.SetBit(i, learnedBit);
            }
        }

        /// <summary>
        ///     Learn a beliefBit by doing
        ///     Random value is used to set the new value
        /// </summary>
        /// <param name="model"></param>
        /// <param name="beliefBit"></param>
        public void Learn(RandomGenerator model, byte beliefBit)
        {
            var bit = BeliefBits.GetBit(beliefBit);
            switch (model)
            {
                case RandomGenerator.RandomUniform:
                    bit += ContinuousUniform.Sample(RangeMin, RangeMax);
                    if (bit < RangeMin)
                    {
                        bit = RangeMin;
                    }

                    if (bit > RangeMax)
                    {
                        bit = RangeMax;
                    }

                    break;
                case RandomGenerator.RandomBinary:
                    bit = DiscreteUniform.Sample(RangeMin, RangeMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }

            BeliefBits.SetBit(beliefBit, bit);
        }

        /// <summary>
        ///     Get the sum of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetBeliefSum()
        {
            if (BeliefBits is null)
            {
                throw new NullReferenceException(nameof(BeliefBits));
            }

            return BeliefBits.GetSum();
        }
    }
}