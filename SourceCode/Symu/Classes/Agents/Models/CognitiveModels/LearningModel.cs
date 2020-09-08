#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Organization;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA;
using Symu.DNA.Networks;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.DNA.Networks.TwoModesNetworks;
using Symu.Repository.Entity;
using static Symu.Common.Constants;

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
        private readonly RandomGenerator _model;
        private readonly IAgentId _id;
        private readonly InternalCharacteristics _internalCharacteristics;
        private readonly KnowledgeNetwork _knowledgeNetwork;
        private readonly AgentKnowledgeNetwork _agentKnowledgeNetwork;
        private readonly byte _randomLevel;
        /// <summary>
        ///     Accumulates all learning of the agent during the simulation
        /// </summary>
        public float CumulativeLearning { get; private set; }
        /// <summary>
        ///     Percentage of all learning of the agent for all knowledge during the simulation
        /// </summary>
        public float PercentageLearning
        {
            get
            {
                float percentage = 0;
                var sum = CumulativeLearning;
                var potential = GetKnowledgePotential();
                if (potential > Tolerance)
                {
                    percentage = 100 * sum / potential;
                }

                return percentage;
            }
        }
        /// <summary>
        ///     Get the maximum potential knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgePotential()
        {
            return Expertise.GetAgentKnowledges<AgentKnowledge>().Sum(l => l.GetKnowledgePotential());
        }

        public LearningModel(IAgentId agentId, OrganizationModels models, MetaNetwork network,
            CognitiveArchitecture cognitiveArchitecture, RandomGenerator model)
        {
            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            models.Learning.CopyTo(this);
            _id = agentId;
            TasksAndPerformance = cognitiveArchitecture.TasksAndPerformance;
            _internalCharacteristics = cognitiveArchitecture.InternalCharacteristics;
            _knowledgeNetwork = network.Knowledge;
            _agentKnowledgeNetwork = network.AgentKnowledge;
            _randomLevel = models.RandomLevelValue;
            if (!cognitiveArchitecture.InternalCharacteristics.CanLearn || !cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge)
            {
                // Agent is not concerned by this model
                On = false;
            }

            Expertise = _agentKnowledgeNetwork.Exists(_id) ? _agentKnowledgeNetwork.GetAgentExpertise(_id): null;
            _model = model;
        }

        public TasksAndPerformance TasksAndPerformance { get; set; }

        private AgentExpertise Expertise { get; }
        //{
        //    get
        //    {
        //        if (!_networkKnowledges.Exists(_id))
        //        {
        //            return null;
        //        }

        //        var expertise = _networkKnowledges.GetAgentExpertise(_id);
        //        expertise.OnAfterLearning += AfterLearning;
        //        return expertise;
        //    }
        //}

        /// <summary>
        ///     EventHandler triggered after learning a new information
        /// </summary>
        public event EventHandler<LearningEventArgs> OnAfterLearning;


        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     Knowledge is stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBits">the knowledge Bits to learn</param>
        /// <param name="maxRateLearnable">Maximum rate learnable from the message, depending on the medium used</param>
        /// <param name="step"></param>
        public void Learn(IId knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
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
        public void Learn(IId knowledgeId, Bits knowledgeBits, float maxRateLearnable, float minimumKnowledge,
            short timeToLive, ushort step)
        {
            if (!IsAgentOn())
            {
                return;
            }
            if (knowledgeId == null)
            {
                throw new ArgumentNullException(nameof(knowledgeId));
            }

            if (knowledgeId.IsNull || knowledgeBits == null || Math.Abs(TasksAndPerformance.LearningRate) < Tolerance ||
                Math.Abs(maxRateLearnable) < Tolerance)
            {
                return;
            }

            if (!knowledgeId.IsNull && knowledgeBits == null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            var agentKnowledge = Expertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId);
            Learn(knowledgeBits, maxRateLearnable, agentKnowledge, step);
        }

        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     agentKnowledge is updated, but not stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeBits">KnowledgeBits from other agent, source of learning</param>
        /// <param name="maxRateLearnable"></param>
        /// <param name="agentKnowledge"></param>
        /// <param name="step"></param>
        public void Learn(Bits knowledgeBits, float maxRateLearnable, AgentKnowledge agentKnowledge, ushort step)
        {
            if (!IsAgentOn())
            {
                return;
            }
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
                AgentKnowledgeLearn(agentKnowledge, i, learning, step);
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
        public float Learn(IId knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive, ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return AgentKnowledgeLearn(Expertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId), knowledgeBit, NextLearning(), step);
        }

        /// <summary>
        ///     Agent learn _knowledgeBits at a learningRate
        ///     OnAfterLearning event is triggered if learning occurs, you can subscribe to this event to treat the new learning
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="index"></param>
        /// <param name="learningRate"></param>
        /// <param name="step"></param>
        /// <returns>The real learning value</returns>
        public float AgentKnowledgeLearn(AgentKnowledge agentKnowledge, byte index, float learningRate, ushort step)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (Math.Abs(learningRate) < Tolerance)
            {
                return 0;
            }

            var realLearning = agentKnowledge.KnowledgeBits.UpdateBit(index, learningRate, step);
            CumulativeLearning += realLearning;
            if (realLearning > Tolerance)
            {
                var learningEventArgs = new LearningEventArgs(agentKnowledge.KnowledgeId, index, realLearning);
                OnAfterLearning?.Invoke(this, learningEventArgs);
            }

            return realLearning;
        }

        /// <summary>
        ///     BeInfluenced by doing a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float LearnByDoing(IId knowledgeId, byte knowledgeBit, ushort step)
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
        public float LearnByDoing(IId knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return AgentKnowledgeLearn(Expertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId), knowledgeBit, NextLearningByDoing(), step);
        }

        public float NextLearning()
        {
            // LearningModel.IsAgentOn is tested at each learning: it is not binary, sometimes you learn, sometimes not
            if (!IsAgentOn())
            {
                return 0;
            }

            var stdDev =
                LearningStandardDeviationValue(TasksAndPerformance.LearningStandardDeviation);
            return TasksAndPerformance.LearningRate * Normal.Sample(1, stdDev * _randomLevel);
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

            var stdDev =
                LearningStandardDeviationValue(TasksAndPerformance.LearningStandardDeviation);
            return TasksAndPerformance.LearningByDoingRate * Normal.Sample(1, stdDev * _randomLevel);
        }
        /// <summary>
        ///     Agent don't have still this Knowledge, it's time to create one
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="knowledgeId"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public void LearnNewKnowledge(IAgentId agentId, IId knowledgeId, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (_agentKnowledgeNetwork.Exists(agentId, knowledgeId))
            {
                return;
            }

            var agentKnowledge = new AgentKnowledge(knowledgeId, KnowledgeLevel.NoKnowledge, minimumKnowledge, timeToLive);
            _agentKnowledgeNetwork.Add(agentId, agentKnowledge);
            var knowledge = _knowledgeNetwork.Get<Knowledge>(knowledgeId);
            agentKnowledge.InitializeKnowledge(knowledge.Length, _model, KnowledgeLevel.NoKnowledge, step);
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
        ///     Subscribe to this event to treat the new learning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AfterLearning(object sender, LearningEventArgs e)
        {
            OnAfterLearning?.Invoke(this, e);
        }
    }
}