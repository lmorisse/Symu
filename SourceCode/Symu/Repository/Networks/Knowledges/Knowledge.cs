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

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Describe an area of knowledge
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class Knowledge
    {
        public Knowledge(ushort id, string name, byte length)
        {
            Id = id;
            Name = name;
            if (length > Bits.MaxBits)
            {
                throw new ArgumentOutOfRangeException("Length should be <= " + Bits.MaxBits);
            }

            Length = length;
        }

        /// <summary>
        ///     For the moment only an id
        /// </summary>
        public ushort Id { get; }

        public string Name { get; }

        /// <summary>
        ///     Each area of knowledge is represented by a collection of KnowledgeBits
        ///     The size define the length of the collection
        ///     each bit represent a single atomic fact
        ///     size range [0; 10]
        /// </summary>
        public byte Length { get; }

        public override bool Equals(object obj)
        {
            return obj is Knowledge knowledge
                   && Id == knowledge.Id;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Transform KnowledgeLevel into a value between [0;1]
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static float GetValueFromKnowledgeLevel(KnowledgeLevel level)
        {
            return ContinuousUniform.Sample(GetMinFromKnowledgeLevel(level), GetMaxFromKnowledgeLevel(level));
        }

        public static float GetMaxFromKnowledgeLevel(KnowledgeLevel level)
        {
            switch (level)
            {
                case KnowledgeLevel.NoKnowledge:
                    return 0;
                case KnowledgeLevel.BasicKnowledge:
                    return 0.5F;
                case KnowledgeLevel.Foundational:
                    return 0.6F;
                case KnowledgeLevel.Intermediate:
                    return 0.7F;
                case KnowledgeLevel.FullProficiency:
                    return 0.8F;
                case KnowledgeLevel.Expert:
                    return 0.9F;
                case KnowledgeLevel.FullKnowledge:
                    return 1.0F;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public static float GetMinFromKnowledgeLevel(KnowledgeLevel level)
        {
            switch (level)
            {
                case KnowledgeLevel.NoKnowledge:
                    return 0;
                case KnowledgeLevel.BasicKnowledge:
                    return 0.2F;
                case KnowledgeLevel.Foundational:
                    return 0.3F;
                case KnowledgeLevel.Intermediate:
                    return 0.4F;
                case KnowledgeLevel.FullProficiency:
                    return 0.5F;
                case KnowledgeLevel.Expert:
                    return 0.6F;
                case KnowledgeLevel.FullKnowledge:
                    return 1F;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        ///     Given a KnowledgeModel and a KnowledgeLevel
        ///     return the knowledgeBits for the agent: an array fill of random binaries
        ///     representing the detailed knowledge of an agent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="knowledgeLevel"></param>
        /// <returns></returns>
        public float[] InitializeBits(RandomGenerator model, KnowledgeLevel knowledgeLevel)
        {
            float[] knowledgeBits;
            switch (model)
            {
                case RandomGenerator.RandomUniform:
                {
                    float min;
                    float max;

                    switch (knowledgeLevel)
                    {
                        case KnowledgeLevel.Random:
                            min = 0;
                            max = 1;
                            break;
                        default:
                            min = GetMinFromKnowledgeLevel(knowledgeLevel);
                            max = GetMaxFromKnowledgeLevel(knowledgeLevel);
                            break;
                    }

                    knowledgeBits = ContinuousUniform.Samples(Length, min, max);
                    if (Math.Abs(min - max) < Constants.Tolerance)
                    {
                        return knowledgeBits;
                    }

                    for (byte i = 0; i < knowledgeBits.Length; i++)
                    {
                        if (knowledgeBits[i] < min * (1 + 0.05))
                        {
                            // In randomUniform, there is quasi no bit == 0. But in reality, there are knowledgeBit we ignore.
                            // We force the lowest (Min +5%) knowledgeBit to 0  
                            knowledgeBits[i] = 0;
                        }
                    }

                    break;
                }
                case RandomGenerator.RandomBinary:
                {
                    switch (knowledgeLevel)
                    {
                        case KnowledgeLevel.Random:
                            knowledgeBits = ContinuousUniform.FilteredSamples(Length, 0, 1);
                            break;
                        default:
                            var mean = 1 - GetValueFromKnowledgeLevel(knowledgeLevel);
                            knowledgeBits = ContinuousUniform.FilteredSamples(Length, mean);
                            break;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }

            return knowledgeBits;
        }

        /// <summary>
        ///     Get a random knowledge Bit (index)index
        /// </summary>
        /// <returns></returns>
        public byte GetRandomBitIndex()
        {
            return DiscreteUniform.SampleToByte(0, (byte) (Length - 1));
        }
    }
}