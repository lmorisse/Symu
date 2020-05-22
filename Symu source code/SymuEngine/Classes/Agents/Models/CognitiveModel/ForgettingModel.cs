#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will forget
    ///     ForgettingEntity enable or not this mechanism for all the agents during the symu
    ///     The ForgettingModel initialize the real value of the agent's forgetting parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    /// <remarks>In addition, we have the MacroLearningModel</remarks>
    public class ForgettingModel : ModelEntity
    {
        private readonly AgentId _id;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly NetworkKnowledges _network;
        private readonly byte _randomLevel;
        private bool _isAgentOnToday;

        public ForgettingModel(ModelEntity entity, CognitiveArchitecture cognitive, byte randomLevel) :
            base(entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (cognitive is null)
            {
                throw new ArgumentNullException(nameof(cognitive));
            }

            InternalCharacteristics = cognitive.InternalCharacteristics;
            _knowledgeAndBeliefs = cognitive.KnowledgeAndBeliefs;
            _randomLevel = randomLevel;
            if (!InternalCharacteristics.CanForget)
            {
                // Agent is not concerned by this model
                On = false;
            }
        }

        public ForgettingModel(ModelEntity entity, CognitiveArchitecture cognitive, byte randomLevel,
            NetworkKnowledges network, AgentId id) :
            this(entity, cognitive, randomLevel)
        {
            _network = network;
            _id = id;
        }

        public ForgettingModel(AgentId agentId, OrganizationModels models, CognitiveArchitecture cognitive,
            NetworkKnowledges network) :
            this(models.Forgetting, cognitive, models.RandomLevelValue)
        {
            _network = network;
            _id = agentId;
        }

        public InternalCharacteristics InternalCharacteristics { get; set; }

        private AgentExpertise Expertise => _network.GetAgentExpertise(_id);
        public AgentExpertise ForgettingExpertise { get; } = new AgentExpertise();

        /// <summary>
        ///     Return the next forgetting rate
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextRate if model is On</returns>
        public float NextMean()
        {
            if (InternalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(InternalCharacteristics));
            }

            // IsAgentOn is tested at each learning: it is not binary, sometimes you forget, sometimes not
            if (!IsAgentOn())
            {
                return 0;
            }

            var forgettingStandardDeviation =
                ForgettingStandardDeviationValue(InternalCharacteristics.ForgettingStandardDeviation);
            return Normal.Sample(InternalCharacteristics.ForgettingMean, forgettingStandardDeviation * _randomLevel);
        }

        /// <summary>
        ///     Return the next forgetting rate
        ///     Use at the initialization of the agent
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextPartialRate if model is On</returns>
        public float NextRate()
        {
            if (InternalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(InternalCharacteristics));
            }

            if (!IsAgentOn())
            {
                return 0;
            }

            return InternalCharacteristics.PartialForgetting ? InternalCharacteristics.PartialForgettingRate : 1;
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
                case GenericLevel.Medium:
                    return 0.15F;
                case GenericLevel.High:
                    return 0.2F;
                case GenericLevel.VeryHigh:
                    return 0.25F;
                case GenericLevel.Complete:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        ///     Set the knowledges of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the day
        /// </summary>
        public void InitializeForgettingProcess()
        {
            // Check if forgetting process is On
            _isAgentOnToday = IsAgentOn() && _knowledgeAndBeliefs.HasKnowledge;
            if (!_isAgentOnToday)
            {
                return;
            }

            if (Expertise == null)
            {
                throw new NullReferenceException(nameof(Expertise));
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
        /// <param name="step"></param>
        public void FinalizeForgettingProcess(ushort step)
        {
            // Check if forgetting process is On
            if (!_isAgentOnToday)
            {
                return;
            }

            foreach (var forget in ForgettingExpertise.List)
            {
                FinalizeForgettingKnowledge(forget, step);
            }
        }

        public void FinalizeForgettingKnowledge(AgentKnowledge forget, ushort step)
        {
            if (forget is null)
            {
                throw new ArgumentNullException(nameof(forget));
            }

            var forgetBits = forget.CloneBits();
            var agentKnowledge = Expertise.GetKnowledge(forget.KnowledgeId);
            switch (InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    for (byte i = 0; i < forgetBits.Length; i++)
                    {
                        agentKnowledge.Forget(i, forgetBits.GetBit(i), step);
                    }

                    break;
                case ForgettingSelectingMode.Oldest:
                    agentKnowledge.ForgetOldest(NextRate(), step);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Set the KnowledgeBits of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the step
        /// </summary>
        public AgentKnowledge InitializeForgettingKnowledge(AgentKnowledge knowledge)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            float[] forgettingKnowledgeBits;
            switch (InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    forgettingKnowledgeBits = InitializeForgettingKnowledgeRandom(knowledge, NextRate());
                    break;
                case ForgettingSelectingMode.Oldest:
                    forgettingKnowledgeBits = Bits.Initialize(knowledge.Length, 0F);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new AgentKnowledge(knowledge.KnowledgeId, forgettingKnowledgeBits, 0, -1, 0);
        }

        /// <summary>
        ///     Initialize the forgetting knowledge process with a random Selecting mode
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="nextForgettingRate"></param>
        /// <returns></returns>
        public float[] InitializeForgettingKnowledgeRandom(AgentKnowledge knowledge, float nextForgettingRate)
        {
            if (knowledge is null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var forgettingKnowledgeBits = ContinuousUniform.Samples(knowledge.Length, 0, 1);
            var threshold = NextMean();
            for (byte i = 0; i < knowledge.Length; i++)
            {
                forgettingKnowledgeBits[i] = forgettingKnowledgeBits[i] < threshold ? nextForgettingRate : 0;
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

        /// <summary>
        ///     As the worker will work on specific TaskKnowledgeBits, they can't be forget
        ///     ForgettingExpertiseOfTheDay is updated
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="taskBits"></param>
        public void UpdateForgettingProcess(ushort knowledgeId, TaskKnowledgeBits taskBits)
        {
            if (taskBits is null)
            {
                throw new ArgumentNullException(nameof(taskBits));
            }

            // Check if forgetting process is On
            if (!_isAgentOnToday)
            {
                return;
            }

            UpdateForgettingProcess(knowledgeId, taskBits.GetMandatory());
            UpdateForgettingProcess(knowledgeId, taskBits.GetRequired());
        }
    }
}