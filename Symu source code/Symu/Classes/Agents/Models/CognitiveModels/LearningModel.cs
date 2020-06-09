﻿#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Repository.Networks.Knowledges;
using Symu.Tools.Math.ProbabilityDistributions;
using static Symu.Tools.Constants;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will learn
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The LearningModel initialize the real value of the agent's learning parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class LearningModel : ModelEntity
    {
        private readonly AgentId _id;
        private readonly InternalCharacteristics _internalCharacteristics;
        private readonly NetworkKnowledges _networkKnowledges;

        private readonly byte _randomLevel;

        public TasksAndPerformance TasksAndPerformance { get; set; }

        private AgentExpertise Expertise
        {
            get
            {
                if (!_networkKnowledges.Exists(_id))
                {
                    return null;
                }

                var expertise = _networkKnowledges.GetAgentExpertise(_id);
                expertise.OnAfterLearning += AfterLearning;
                return expertise;
            }
        }
        /// <summary>
        ///     EventHandler triggered after learning a new information
        /// </summary>
        public event EventHandler<LearningEventArgs> OnAfterLearning;

        public LearningModel(AgentId agentId, OrganizationModels models, NetworkKnowledges networkKnowledges,
            CognitiveArchitecture cognitiveArchitecture)
        {
            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            models.Learning.CopyTo(this);
            _id = agentId;
            TasksAndPerformance = cognitiveArchitecture.TasksAndPerformance;
            _internalCharacteristics = cognitiveArchitecture.InternalCharacteristics;
            _networkKnowledges = networkKnowledges;
            _randomLevel = models.RandomLevelValue;
            if (!cognitiveArchitecture.InternalCharacteristics.CanLearn)
            {
                // Agent is not concerned by this model
                On = false;
            }
        }


        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     Knowledge is stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBits">the knowledge Bits to learn</param>
        /// <param name="maxRateLearnable">Maximum rate learnable from the message, depending on the medium used</param>
        /// <param name="step"></param>
        public void Learn(ushort knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
        {
            Learn(knowledgeId, knowledgeBits, maxRateLearnable, _internalCharacteristics.MinimumRemainingKnowledge,
                _internalCharacteristics.TimeToLive, step);
        }

        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     Knowledge is stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBits">the knowledge Bits to learn</param>
        /// <param name="maxRateLearnable">Maximum rate learnable from the message, depending on the medium used</param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public void Learn(ushort knowledgeId, Bits knowledgeBits, float maxRateLearnable, float minimumKnowledge,
            short timeToLive, ushort step)
        {
            if (knowledgeId == 0 || knowledgeBits == null || Math.Abs(TasksAndPerformance.LearningRate) < Tolerance ||
                Math.Abs(maxRateLearnable) < Tolerance)
            {
                return;
            }

            if (knowledgeId > 0 && knowledgeBits == null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            _networkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            var agentKnowledge = Expertise.GetKnowledge(knowledgeId);
            Learn(knowledgeBits, maxRateLearnable, agentKnowledge, step);
        }

        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     agentKnowledge is updated, but not stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeBits"></param>
        /// <param name="maxRateLearnable"></param>
        /// <param name="agentKnowledge"></param>
        /// <param name="step"></param>
        public void Learn(Bits knowledgeBits, float maxRateLearnable, AgentKnowledge agentKnowledge, ushort step)
        {
            if (knowledgeBits is null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var learningRate = NextLearning();
            if (Math.Abs(learningRate * maxRateLearnable) < Tolerance)
            {
                return;
            }

            for (byte i = 0; i < knowledgeBits.Length; i++)
                // other agent must have more knowledge bit than the agent
            {
                if (!(knowledgeBits.GetBit(i) > 0) || !(knowledgeBits.GetBit(i) >= agentKnowledge.GetKnowledgeBit(i)))
                {
                    continue;
                }

                var learning = Math.Min(knowledgeBits.GetBit(i), learningRate * maxRateLearnable);
                agentKnowledge.Learn(i, learning, step);
            }
        }

        /// <summary>
        ///     BeInfluenced a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float Learn(ushort knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive, ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            _networkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return Expertise.GetKnowledge(knowledgeId).Learn(knowledgeBit, NextLearning(), step);
        }

        /// <summary>
        ///     BeInfluenced by doing a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float LearnByDoing(ushort knowledgeId, byte knowledgeBit, ushort step)
        {
            return LearnByDoing(knowledgeId, knowledgeBit, _internalCharacteristics.MinimumRemainingKnowledge,
                _internalCharacteristics.TimeToLive, step);
        }

        /// <summary>
        ///     BeInfluenced by doing a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float LearnByDoing(ushort knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            _networkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return Expertise.GetKnowledge(knowledgeId).Learn(knowledgeBit, NextLearningByDoing(), step);
        }

        public float NextLearning()
        {
            // LearningModel.IsAgentOn is tested at each learning: it is not binary, sometimes you learn, sometimes not
            if (!IsAgentOn())
            {
                return 0;
            }

            var learningStandardDeviation =
                LearningStandardDeviationValue(TasksAndPerformance.LearningStandardDeviation);
            return Normal.Sample(TasksAndPerformance.LearningRate, learningStandardDeviation * _randomLevel);
        }

        /// <summary>
        ///     Return the next learning by doing rate
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextLearningByDoing Rate if model is On</returns>
        public float NextLearningByDoing()
        {
            // LearningModel.IsAgentOn is tested at each learning: it is not binary, sometimes you learn, sometimes not
            if (!IsAgentOn())
            {
                return 0;
            }

            var learningStandardDeviation =
                LearningStandardDeviationValue(TasksAndPerformance.LearningStandardDeviation);
            return Normal.Sample(TasksAndPerformance.LearningByDoingRate, learningStandardDeviation * _randomLevel);
        }

        private static float LearningStandardDeviationValue(GenericLevel level)
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
        ///     OnAfterLearning event is triggered if learning occurs,
        ///     you can subscribe to this event to treat the new learning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AfterLearning(object sender, LearningEventArgs e)
        {
            OnAfterLearning?.Invoke(this, e);
        }
    }
}