#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Describe an area of knowledge
    /// Default implementation of IKnowledge
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class Knowledge: IKnowledge
    {
        public Knowledge(ushort id, string name, byte length): this(new UId(id), name, length)
        {
        }
        public Knowledge(IId id, string name, byte length)
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
        ///     Unique identifier af the knowledge
        /// </summary>
        public IId Id { get; }

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

        public bool Equals(IKnowledge knowledge)
        {
            return knowledge is Knowledge know
                   && Id == know.Id;
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
        public static float GetValueFromKnowledgeLevel(KnowledgeLevel level)
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
        ///     Get a random knowledge Bit (index)index
        /// </summary>
        /// <returns></returns>
        public byte GetRandomBitIndex()
        {
            return DiscreteUniform.SampleToByte(0, (byte) (Length - 1));
        }
    }
}