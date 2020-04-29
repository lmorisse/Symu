#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Common;
using SymuEngine.Repository.Networks.Knowledge;
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture.Forgetting
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will forget
    ///     ForgettingEntity enable or not this mechanism for all the agents during the simulation
    ///     The ForgettingModel initialize the real value of the agent's forgetting parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    /// <remarks>In addition, we have the MacroLearningModel</remarks>
    public class ForgettingModel : ModelEntity
    {
        private readonly AgentId _id;
        private readonly InternalCharacteristics _internalCharacteristics;
        private readonly NetworkKnowledges _network;
        private readonly byte _randomLevel;

        public ForgettingModel(ModelEntity entity, InternalCharacteristics internalCharacteristics, byte randomLevel) :
            base(entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (internalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            entity.CopyTo(this);
            _internalCharacteristics = internalCharacteristics;
            _randomLevel = randomLevel;
        }

        public ForgettingModel(ModelEntity entity, InternalCharacteristics internalCharacteristics, byte randomLevel,
            NetworkKnowledges network, AgentId id) :
            this(entity, internalCharacteristics, randomLevel)
        {
            _network = network;
            _id = id;
        }

        private AgentExpertise Expertise => _network.GetAgentExpertise(_id);
        public AgentExpertise ForgettingExpertise { get; } = new AgentExpertise();

        /// <summary>
        ///     Return the next forgetting rate
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextRate if model is On</returns>
        public float NextMean()
        {
            if (_internalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(_internalCharacteristics));
            }

            if (!On)
            {
                return 0;
            }

            var forgettingStandardDeviation =
                ForgettingStandardDeviationValue(_internalCharacteristics.ForgettingStandardDeviation);
            return Normal.Sample(_internalCharacteristics.ForgettingMean, forgettingStandardDeviation * _randomLevel);
        }

        /// <summary>
        ///     Return the next forgetting rate
        ///     Use at the initialization of the agent
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextPartialRate if model is On</returns>
        public float NextRate()
        {
            if (_internalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(_internalCharacteristics));
            }

            if (!On)
            {
                return 0;
            }

            return _internalCharacteristics.PartialForgetting ? _internalCharacteristics.PartialForgettingRate : 1;
        }

        public static float ForgettingStandardDeviationValue(GenericLevel level)
        {
            switch (level)
            {
                case GenericLevel.None:
                    return 0;
                case GenericLevel.VeryLow:
                    return 0.05F;
                case GenericLevel.Low:
                    return 0.1F;
                //case GenericLevel.Medium:
                default:
                    return 0.15F;
                case GenericLevel.High:
                    return 0.2F;
                case GenericLevel.VeryHigh:
                    return 0.25F;
                case GenericLevel.Complete:
                    return 1;
            }
        }

        /// <summary>
        ///     Set the knowledges of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the day
        /// </summary>
        public void InitializeForgettingProcess()
        {
            if (Expertise == null)
            {
                throw new NullReferenceException(nameof(Expertise));
            }

            // Check if forgetting process is On
            if (!On)
            {
                return;
            }

            ForgettingExpertise.Clear();
            foreach (var knowledge in Expertise.List)
            {
                ForgettingExpertise.Add(InitializeForgettingKnowledge(knowledge));
            }
        }

        /// <summary>
        ///     Finalize the forgetting process of the day by forgetting the knowledges bits
        ///     Use it at the end of the day
        /// </summary>
        public void FinalizeForgettingProcess()
        {
            // Check if forgetting process is On
            if (!On)
            {
                return;
            }

            foreach (var forget in ForgettingExpertise.List)
            {
                FinalizeForgettingKnowledge(forget);
            }
        }

        public void FinalizeForgettingKnowledge(AgentKnowledge forget)
        {
            if (forget is null)
            {
                throw new ArgumentNullException(nameof(forget));
            }

            var forgetBits = forget.CloneBits();
            for (byte i = 0; i < forgetBits.Length; i++)
            {
                var knowledge = Expertise.GetKnowledge(forget.KnowledgeId);
                knowledge.Forget(i, forgetBits.GetBit(i), _internalCharacteristics.MinimumRemainingLevel);
            }
        }

        /// <summary>
        ///     Set the KnowledgeBits of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the day
        /// </summary>
        public AgentKnowledge InitializeForgettingKnowledge(AgentKnowledge knowledge)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            float[] forgettingKnowledgeBits;
            switch (_internalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    forgettingKnowledgeBits =
                        ContinuousUniform.FilteredSamples(knowledge.Length, 1 - NextMean(), NextRate());
                    break;
                //case ForgettingSelectingMode.Oldest:
                default:
                    forgettingKnowledgeBits = InitializeForgettingKnowledgeOldest(knowledge, NextRate());
                    break;
            }

            return new AgentKnowledge(knowledge.KnowledgeId, forgettingKnowledgeBits, 0);
        }

        /// <summary>
        ///     Initialize the forgetting knowledge process with the Oldest Forgetting Selecting mode
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="nextForgettingRate"></param>
        /// <returns></returns>
        public float[] InitializeForgettingKnowledgeOldest(AgentKnowledge knowledge, float nextForgettingRate)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var forgettingKnowledgeBits = Bits.Initialize(knowledge.Length, 0F);
            // Take the maxBits oldest knowledgeBits
            var maxBits = Convert.ToByte(Math.Round(knowledge.Length * NextMean()));
            if (maxBits == 0)
            {
                return forgettingKnowledgeBits;
            }

            var groups = knowledge.KnowledgeBits.GetLastTouched().ToList().OrderBy(x => x).GroupBy(x => x).ToList();
            var count = 0;
            ushort maxLastTouched = 0;
            foreach (var group in groups)
            {
                count += group.Count();
                if (count < maxBits)
                {
                    continue;
                }

                maxLastTouched = group.Key;
                break;
            }

            for (byte i = 0; i < knowledge.Length; i++)
            {
                if (knowledge.KnowledgeBits.GetLastTouched()[i] <= maxLastTouched)
                {
                    forgettingKnowledgeBits[i] = nextForgettingRate;
                }
            }

            return forgettingKnowledgeBits;
        }

        /// <summary>
        ///     As the worker will work on specific taskBits, they can't be forget
        ///     ForgettingExpertiseOfTheDay is updated
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="workingBits"></param>
        public void UpdateForgettingProcess(ushort knowledgeId, byte[] workingBits)
        {
            if (workingBits is null)
            {
                throw new ArgumentNullException(nameof(workingBits));
            }

            var forgettingKnowledge = ForgettingExpertise.GetKnowledge(knowledgeId);
            if (forgettingKnowledge == null)
            {
                return;
            }

            for (byte i = 0; i < workingBits.Length; i++)
                // the KnowledgeBit will not be forgotten today
            {
                forgettingKnowledge.SetKnowledgeBit(workingBits[i], 0, 0);
            }
        }
    }
}