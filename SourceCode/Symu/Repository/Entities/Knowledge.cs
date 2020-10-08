#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     Describe an area of knowledge
    ///     Based on Default implementation KnowledgeEntity enriched by a collection of KnowledgeBits
    ///     The Length define the length of the collection
    ///     Each bit represent a single atomic fact
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class Knowledge : KnowledgeEntity //IKnowledge
    {
        private readonly MainOrganizationModels _models;

        public Knowledge()
        {
        }
        public static Knowledge CreateInstance(GraphMetaNetwork metaNetwork, MainOrganizationModels models, string name, byte length)
        {
            return new Knowledge(metaNetwork, models, name, length);
        }

        public Knowledge(GraphMetaNetwork metaNetwork, MainOrganizationModels models, string name, byte length) : base(
            metaNetwork, name)
        {
            Length = length;
            if (length > Bits.MaxBits)
            {
                throw new ArgumentOutOfRangeException("Length should be <= " + Bits.MaxBits);
            }

            _models = models ?? throw new ArgumentNullException(nameof(models));
            AddAssociatedBelief();
        }

        /// <summary>
        ///     Each area of knowledge is represented by a collection of KnowledgeBits
        ///     The Length define the length of the collection
        ///     each bit represent a single atomic fact
        ///     size range [0; 10]
        /// </summary>
        public byte Length { get; private set; }

        /// <summary>
        ///     A belief may be associated with this knowledge if models.Beliefs is On
        /// </summary>
        public Belief AssociatedBelief { get; private set; }

        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new Knowledge();
            CopyEntityTo(clone);
            return clone;
        }

        public override void CopyEntityTo(IEntity entity)
        {
            base.CopyEntityTo(entity);
            if (!(entity is Knowledge copy))
            {
                return;
            }

            AddAssociatedBelief();
            copy.Length = Length;
            copy.AssociatedBelief = AssociatedBelief;
        }

        private void AddAssociatedBelief()
        {
            if (_models.Beliefs.On && AssociatedBelief == null)
            {
                AssociatedBelief = new Belief(MetaNetwork, this, Length, _models.Generator, _models.BeliefWeightLevel);
            }
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
                case KnowledgeLevel.Random:
                    return ContinuousUniform.Sample(0, 1F);
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
                case KnowledgeLevel.Random:
                    return ContinuousUniform.Sample(0, 1F);
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