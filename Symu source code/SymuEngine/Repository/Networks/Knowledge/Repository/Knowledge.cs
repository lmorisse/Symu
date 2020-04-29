#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Knowledge;
using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Repository.Networks.Knowledge.Repository
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
                //case KnowledgeLevel.FullKnowledge:
                default:
                    return 1.0F;
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
                //case KnowledgeLevel.FullKnowledge:
                default:
                    return 1F;
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
        public float[] GetAgentBits(KnowledgeModel model, KnowledgeLevel knowledgeLevel)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            float[] knowledgeBits;
            if (model.RandomGenerator == RandomGenerator.RandomUniform)
            {
                var min = GetMinFromKnowledgeLevel(knowledgeLevel);
                var max = GetMaxFromKnowledgeLevel(knowledgeLevel);
                knowledgeBits = ContinuousUniform.Samples(Length, min, max);
                for (byte i = 0; i < knowledgeBits.Length; i++)
                {
                    if (knowledgeBits[i] < min * (1 + 0.05))
                    {
                        knowledgeBits[i] = 0;
                    }
                }
            }
            else
            {
                var mean = 1 - GetValueFromKnowledgeLevel(knowledgeLevel);
                knowledgeBits = ContinuousUniform.FilteredSamples(Length, mean);
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
    }
}