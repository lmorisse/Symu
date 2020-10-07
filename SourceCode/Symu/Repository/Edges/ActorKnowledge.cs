#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.OrgMod.Edges;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks;
using Symu.Repository.Entities;

#endregion

namespace Symu.Repository.Edges
{
    /// <summary>
    ///     Describe the Knowledge of an agent :
    ///     KnowledgeId, KnowledgeLevel, KnowledgeBits
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class ActorKnowledge : EntityKnowledge 
    {
        /// <summary>
        ///     Constructor used by WorkerCognitiveAgent for ForgettingKnowledge
        /// </summary>
        /// <param name="network"></param>
        /// <param name="actorId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        public ActorKnowledge(TwoModesNetwork<IEntityKnowledge> network, IAgentId actorId, IAgentId knowledgeId, KnowledgeBits knowledgeBits) : base(network, actorId,
            knowledgeId)
        {
            KnowledgeBits = knowledgeBits;
            Length = KnowledgeBits?.Length ?? 0;
        }        
        
        /// <summary>
        ///     Constructor used by Agent.Cognitive for ForgettingKnowledge
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public ActorKnowledge(IAgentId actorId, IAgentId knowledgeId, float[] knowledgeBits, float minimumKnowledge,
            short timeToLive,
            ushort step = 0) : base(actorId, knowledgeId)
        {
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
            KnowledgeBits.SetBits(knowledgeBits, step);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     Constructor used by Agent.Cognitive for ForgettingKnowledge
        /// </summary>
        /// <param name="network"></param>
        /// <param name="actorId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public ActorKnowledge(TwoModesNetwork<IEntityKnowledge> network, IAgentId actorId, IAgentId knowledgeId, float[] knowledgeBits, float minimumKnowledge,
            short timeToLive,
            ushort step = 0) : base(network, actorId, knowledgeId)
        {
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
            KnowledgeBits.SetBits(knowledgeBits, step);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     Constructor based on the knowledge Id and the knowledge Level.
        ///     KnowledgeBits is not yet initialized.
        ///     NetworkKnowledges.InitializeAgentKnowledge must be called to initialized KnowledgeBits
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public ActorKnowledge(IAgentId actorId, IAgentId knowledgeId, KnowledgeLevel level, float minimumKnowledge,
            short timeToLive) : base(actorId, knowledgeId)
        {
            KnowledgeLevel = level;
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     Constructor based on the knowledge Id and the knowledge Level.
        ///     KnowledgeBits is not yet initialized.
        ///     NetworkKnowledges.InitializeAgentKnowledge must be called to initialized KnowledgeBits
        /// </summary>
        /// <param name="network"></param>
        /// <param name="actorId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public ActorKnowledge(TwoModesNetwork<IEntityKnowledge> network, IAgentId actorId, IAgentId knowledgeId, KnowledgeLevel level, float minimumKnowledge,
            short timeToLive) : base(network, actorId, knowledgeId)
        {
            KnowledgeLevel = level;
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     The value used to feed the matrix network
        ///     For a binary matrix network, the value is 1
        /// </summary>
        public override float Weight => GetKnowledgeSum();

        public KnowledgeBits KnowledgeBits { get; private set; }
        public KnowledgeLevel KnowledgeLevel { get; set; }

        /// <summary>
        ///     If agent has a knowledgeBit, and the forgetting model is on
        ///     Minimum knowledge is the minimum the agent can't forget during the simulation for this KnowledgeBit.
        ///     Range[0;1]
        /// </summary>
        public float MinimumKnowledge { get; set; }

        /// <summary>
        ///     When ForgettingSelectingMode.Oldest is selected, knowledge are forget based on their timeToLive attribute
        ///     -1 for unlimited time to live
        /// </summary>
        public short TimeToLive { get; set; }

        public byte Length { get; private set; } //=> KnowledgeBits?.Length ?? 0;

        /// <summary>
        ///     Initialize KnowledgeBits with a array filled of 0
        /// </summary>
        public void InitializeWith0(byte length, ushort step)
        {
            KnowledgeBits = new KnowledgeBits(MinimumKnowledge, TimeToLive);
            KnowledgeBits.InitializeWith0(length, step);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits
        ///     so that consumers of this library cannot change its contents
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits CloneBits()
        {
            return KnowledgeBits.Clone();
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits filtered by minimumKnowledge
        ///     if a KnowledgeBit inferior minimumKnowledge then KnowledgeBit = 0
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits CloneWrittenKnowledgeBits(float minimumKnowledge)
        {
            var clone = KnowledgeBits.Clone();

            if (clone.IsNull)
            {
                return null;
            }

            for (byte i = 0; i < clone.Length; i++)
                // intentionally strictly < 
            {
                if (clone.GetBit(i) < minimumKnowledge)
                {
                    clone.SetBit(i, 0);
                }
            }

            return clone;
        }

        /// <summary>
        ///     Get the knowledgeBit at the index i
        /// </summary>
        /// <param name="index"></param>
        /// <returns>-1 if knowledgeBits == null</returns>
        public float GetKnowledgeBit(byte index)
        {
            return KnowledgeBits.GetBit(index);
        }

        /// <summary>
        ///     Get the sum of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetKnowledgeSum()
        {
            return KnowledgeBits.GetSum();
        }

        /// <summary>
        ///     Get the maximum potential of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetKnowledgePotential()
        {
            return KnowledgeBits.Length;
        }

        /// <summary>
        ///     Given a KnowledgeModel and a KnowledgeLevel
        ///     return the knowledgeBits for the agent: an array fill of random binaries
        ///     representing the detailed knowledge of an agent
        /// </summary>
        /// <param name="length"></param>
        /// <param name="model"></param>
        /// <param name="knowledgeLevel"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public void InitializeKnowledge(byte length, RandomGenerator model, KnowledgeLevel knowledgeLevel, ushort step)
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
                            min = Knowledge.GetMinFromKnowledgeLevel(knowledgeLevel);
                            max = Knowledge.GetMaxFromKnowledgeLevel(knowledgeLevel);
                            break;
                    }

                    knowledgeBits = ContinuousUniform.Samples(length, min, max);
                    if (Math.Abs(min - max) < Constants.Tolerance)
                    {
                        SetKnowledgeBits(knowledgeBits, step);
                        return;
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
                            knowledgeBits = ContinuousUniform.FilteredSamples(length, 0, 1);
                            break;
                        default:
                            var mean = 1 - Knowledge.GetValueFromKnowledgeLevel(knowledgeLevel);
                            knowledgeBits = ContinuousUniform.FilteredSamples(length, mean);
                            break;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }

            SetKnowledgeBits(knowledgeBits, step);
        }

        public void SetKnowledgeBits(float[] knowledgeBits, ushort step)
        {
            if (KnowledgeBits is null)
            {
                KnowledgeBits = new KnowledgeBits(MinimumKnowledge, TimeToLive);
            }

            KnowledgeBits.SetBits(knowledgeBits, step);
            Length = KnowledgeBits.Length;
        }

        /// <summary>
        ///     Agent forget _knowledgeBits at a forgetRate coming from ForgettingModel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public void SetKnowledgeBit(byte index, float value, ushort step)
        {
            KnowledgeBits.SetBit(index, value, step);
        }
    }
}