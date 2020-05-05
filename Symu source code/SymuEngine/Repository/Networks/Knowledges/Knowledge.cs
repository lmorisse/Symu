#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuTools.Classes.Algorithm;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Repository.Networks.Knowledges
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
        public float[] GetAgentBits(RandomGenerator model, KnowledgeLevel knowledgeLevel)
        {
            float[] knowledgeBits;
            switch (model)
            {
                case RandomGenerator.RandomUniform:
                {
                    var min = GetMinFromKnowledgeLevel(knowledgeLevel);
                    var max = GetMaxFromKnowledgeLevel(knowledgeLevel);
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
                    var mean = 1 - GetValueFromKnowledgeLevel(knowledgeLevel);
                    knowledgeBits = ContinuousUniform.FilteredSamples(Length, mean);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }

            return knowledgeBits;
        }

        /// <summary>
        ///     Given a KnowledgeModel and a KnowledgeLevel
        ///     return the required knowledgeBits index for the task: an array fill of random index of the GetBits
        ///     representing the detailed knowledge of an agent
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <example>
        ///     given a knowledge of size 10 :
        ///     GetAgentBits = [0110110000]
        ///     GetTaskRequiredBits = [359] : index 3, 5 and 9 are required
        /// </example>
        public byte[] GetTaskRequiredBits(float level)
        {
            var numberRequiredBits = Convert.ToByte(Math.Round(MurphyTask.RequiredBitsRatio(level) * Length));
            return DiscreteUniform.SamplesToByte(numberRequiredBits, Length - 1);
        }

        public byte[] GetTaskMandatoryBits(MurphyTask model, float level)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var numberMandatoryBits = Convert.ToByte(Math.Round(model.MandatoryBitsRatio(level) * Length));
            return DiscreteUniform.SamplesToByte(numberMandatoryBits, Length - 1);
        }

        /// <summary>
        ///     Get a random knowledge Bit (index)index
        /// </summary>
        /// <returns></returns>
        public byte GetRandomBit()
        {
            return DiscreteUniform.SampleToByte(0, (byte) (Length - 1));
        }
    }
}