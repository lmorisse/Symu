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
using Symu.Common.Interfaces;

using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA;
using Symu.DNA.Edges;
using Symu.DNA.Entities;
using Symu.DNA.GraphNetworks;
using Symu.DNA.GraphNetworks.TwoModesNetworks;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using static Symu.Common.Constants;
using ActorKnowledge = Symu.Repository.Edges.ActorKnowledge;

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
        private readonly IAgentId _agentId;
        private readonly InternalCharacteristics _internalCharacteristics;
        private readonly OneModeNetwork _knowledgeNetwork;
        private readonly TwoModesNetwork<IEntityKnowledge> _entityKnowledgeNetwork;
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
            return _entityKnowledgeNetwork.EdgesFilteredBySource<ActorKnowledge>(_agentId).Sum(l => l.GetKnowledgePotential());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="models"></param>
        /// <param name="knowledgeNetwork"></param>
        /// <param name="entityKnowledgeNetwork">ActorKnowledgeNetwork, ResourceKnowledgeNetwork depending on the agent</param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="model"></param>
        /// <param name="randomLevel"></param>
        public LearningModel(IAgentId agentId, OrganizationModels models, OneModeNetwork knowledgeNetwork, TwoModesNetwork<IEntityKnowledge> entityKnowledgeNetwork,
            CognitiveArchitecture cognitiveArchitecture, RandomGenerator model, byte randomLevel)
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
            _agentId = agentId;
            TasksAndPerformance = cognitiveArchitecture.TasksAndPerformance;
            _internalCharacteristics = cognitiveArchitecture.InternalCharacteristics;
            _knowledgeNetwork = knowledgeNetwork;
            _entityKnowledgeNetwork = entityKnowledgeNetwork ?? throw new ArgumentNullException(nameof(entityKnowledgeNetwork));
            _randomLevel = randomLevel;
            if (!cognitiveArchitecture.InternalCharacteristics.CanLearn || !cognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge)
            {
                // Agent is not concerned by this model
                On = false;
            }
            _model = model;
        }

        public TasksAndPerformance TasksAndPerformance { get; set; }

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
        public void Learn(IAgentId knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
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
        public void Learn(IAgentId knowledgeId, Bits knowledgeBits, float maxRateLearnable, float minimumKnowledge,
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

            LearnNewKnowledge(_agentId, knowledgeId, minimumKnowledge, timeToLive, step);
            var agentKnowledge = _entityKnowledgeNetwork.Edge<ActorKnowledge>(_agentId,knowledgeId);
            Learn(knowledgeBits, maxRateLearnable, agentKnowledge, step);
        }

        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     agentKnowledge is updated, but not stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeBits">KnowledgeBits from other agent, source of learning</param>
        /// <param name="maxRateLearnable"></param>
        /// <param name="actorKnowledge"></param>
        /// <param name="step"></param>
        public void Learn(Bits knowledgeBits, float maxRateLearnable, ActorKnowledge actorKnowledge, ushort step)
        {
            if (!IsAgentOn())
            {
                return;
            }
            if (knowledgeBits is null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            if (actorKnowledge is null)
            {
                throw new ArgumentNullException(nameof(actorKnowledge));
            }

            var learningRate = NextLearning();
            if (Math.Abs(learningRate * maxRateLearnable) < Tolerance)
            {
                return;
            }

            for (byte i = 0; i < knowledgeBits.Length; i++)
                // other agent must have more knowledge bit than the agent
            {
                if (!(knowledgeBits.GetBit(i) > 0) || !(knowledgeBits.GetBit(i) >= actorKnowledge.GetKnowledgeBit(i)))
                {
                    continue;
                }

                var learning = Math.Min(knowledgeBits.GetBit(i), learningRate * maxRateLearnable);
                AgentKnowledgeLearn(actorKnowledge, i, learning, step);
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
        public float Learn(IAgentId knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive, ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            LearnNewKnowledge(_agentId, knowledgeId, minimumKnowledge, timeToLive, step);
            return AgentKnowledgeLearn(_entityKnowledgeNetwork.Edge<ActorKnowledge>(_agentId,knowledgeId), knowledgeBit, NextLearning(), step);
        }

        /// <summary>
        ///     Agent learn _knowledgeBits at a learningRate
        ///     OnAfterLearning event is triggered if learning occurs, you can subscribe to this event to treat the new learning
        /// </summary>
        /// <param name="actorKnowledge"></param>
        /// <param name="index"></param>
        /// <param name="learningRate"></param>
        /// <param name="step"></param>
        /// <returns>The real learning value</returns>
        public float AgentKnowledgeLearn(ActorKnowledge actorKnowledge, byte index, float learningRate, ushort step)
        {
            if (actorKnowledge == null)
            {
                throw new ArgumentNullException(nameof(actorKnowledge));
            }

            if (Math.Abs(learningRate) < Tolerance)
            {
                return 0;
            }

            var realLearning = actorKnowledge.KnowledgeBits.UpdateBit(index, learningRate, step);
            CumulativeLearning += realLearning;
            if (realLearning > Tolerance)
            {
                var learningEventArgs = new LearningEventArgs(actorKnowledge.Target, index, realLearning);
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
        public float LearnByDoing(IAgentId knowledgeId, byte knowledgeBit, ushort step)
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
        public float LearnByDoing(IAgentId knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (!IsAgentOn())
            {
                return 0;
            }

            LearnNewKnowledge(_agentId, knowledgeId, minimumKnowledge, timeToLive, step);
            return AgentKnowledgeLearn(_entityKnowledgeNetwork.Edge<ActorKnowledge>(_agentId, knowledgeId), knowledgeBit, NextLearningByDoing(), step);
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
        public void LearnNewKnowledge(IAgentId agentId, IAgentId knowledgeId, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            if (!_knowledgeNetwork.Exists(knowledgeId))
            {
                throw new NullReferenceException(nameof(knowledgeId));
            }
            if (_entityKnowledgeNetwork.Exists(agentId, knowledgeId))
            {
                return;
            }

            var agentKnowledge = new ActorKnowledge(agentId, knowledgeId, KnowledgeLevel.NoKnowledge, minimumKnowledge, timeToLive);
            _entityKnowledgeNetwork.Add(agentKnowledge);
            var knowledge = _knowledgeNetwork.GetEntity<Knowledge>(knowledgeId);
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