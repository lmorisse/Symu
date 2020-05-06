#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Activities;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.ProbabilityDistributions;
using static SymuTools.Algorithm.Constants;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     Tasks & Performance from Construct Software
    ///     Activities
    ///     MultiTasking
    ///     Performs tasks
    ///     learning by doing
    /// </summary>
    /// <remarks>Tasks and Performance from Construct Software</remarks>
    public class TasksAndPerformance
    {
        private readonly AgentId _id;
        private readonly Network _network;
        private readonly byte _randomLevel;

        public TasksAndPerformance()
        {
        }

        public TasksAndPerformance(Network network, AgentId id, byte randomLevel)
        {
            _network = network;
            _id = id;
            _randomLevel = randomLevel;
        }

        public AgentExpertise Expertise => _network.NetworkKnowledges.GetAgentExpertise(_id);

        public MurphyTask TaskModel { get; } = new MurphyTask();

        /// <summary>
        ///     Manage all limits related to Tasks
        /// </summary>
        public TasksLimit TasksLimit { get; set; } = new TasksLimit();

        public void CopyTo(TasksAndPerformance tasksAndPerformance)
        {
            if (tasksAndPerformance is null)
            {
                throw new ArgumentNullException(nameof(tasksAndPerformance));
            }

            tasksAndPerformance.LearningRate = LearningRate;
            tasksAndPerformance.LearningByDoingRate = LearningByDoingRate;
            tasksAndPerformance.CostFactorOfLearningByDoing = CostFactorOfLearningByDoing;
            tasksAndPerformance.LearningStandardDeviation = LearningStandardDeviation;
            tasksAndPerformance.CanPerformTask = CanPerformTask;
            tasksAndPerformance.CanPerformTaskOnWeekEnds = CanPerformTaskOnWeekEnds;
            LearningModel.CopyTo(tasksAndPerformance.LearningModel);
            TasksLimit.CopyTo(tasksAndPerformance.TasksLimit);
        }

        #region Agent Learning

        public ModelEntity LearningModel { get; set; } = new ModelEntity();

        private float _learningRate;

        /// <summary>
        ///     AgentLearningRate define how quickly an agent will learn new knowledge when interacting with other agents
        ///     It impacts the KnowledgeBits of the Agent
        ///     With a rate of 0.01F with no knowledge , an actor become an expert on a specific KnowledgeBit in 100 times
        /// </summary>
        /// <example>
        ///     if AgentLearningRate = 0.01 and the agent is learning on a KnowledgeBit = 0.5 => the upgraded KnowledgeBit =
        ///     0.5 +0.01 = 0.51
        /// </example>
        public float LearningRate
        {
            get => _learningRate;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("LearningRate should be between 0 and 1");
                }

                _learningRate = value;
            }
        }

        private float _learningByDoingRate;

        /// <summary>
        ///     AgentLearningByDoingRate define how quickly an agent will learn new knowledge when performing tasks  without
        ///     interacting with other agents
        ///     This allows agents to learn more about knowledge bits that are partially known.
        ///     It impacts the KnowledgeBits of the Agent
        ///     With a rate of 0.01F with no knowledge , an actor become an expert on a specific KnowledgeBit in 100 times
        /// </summary>
        /// <example>
        ///     if AgentLearningRate = 0.01 and the agent is learning on a KnowledgeBit = 0.5 => the upgraded KnowledgeBit =
        ///     0.5 + 0.01 = 0.51
        /// </example>
        public float LearningByDoingRate
        {
            get => _learningByDoingRate;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("LearningByDoingRate should be between 0 and 1");
                }

                _learningByDoingRate = value;
            }
        }

        private float _costFactorOfLearningByDoing = 1;

        /// <summary>
        ///     Learning by doing is costly. CostOfLearningByDoing define the factor to know the real cost of the task
        /// </summary>
        /// <example>if a task estimated cost = 1 and CostOfLearningByDoing = 2, then the real task cost = 1*2</example>
        public float CostFactorOfLearningByDoing
        {
            get => _costFactorOfLearningByDoing;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("CostFactorOfLearningByDoing should be > 0 ");
                }

                _costFactorOfLearningByDoing = value;
            }
        }

        /// <summary>
        ///     Standard deviation around the LearningRate and LearningByDoingRate
        ///     Default 0.1F
        /// </summary>
        public GenericLevel LearningStandardDeviation { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     Agent learn from an other agent who send KnowledgeBits.
        ///     Knowledge is stored in NetworkKnowledges
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBits">the knowledge Bits to learn</param>
        /// <param name="maxRateLearnable">Maximum rate learnable from the message, depending on the medium used</param>
        /// <param name="internalCharacteristics"></param>
        /// <param name="step"></param>
        public void Learn(ushort knowledgeId, Bits knowledgeBits, float maxRateLearnable,
            InternalCharacteristics internalCharacteristics, ushort step)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            Learn(knowledgeId, knowledgeBits, maxRateLearnable, internalCharacteristics.MinimumRemainingKnowledge,
                internalCharacteristics.TimeToLive, step);
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
            if (knowledgeId == 0 || knowledgeBits == null || Math.Abs(LearningRate) < Tolerance ||
                Math.Abs(maxRateLearnable) < Tolerance)
            {
                return;
            }

            if (knowledgeId > 0 && knowledgeBits == null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
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
                if (knowledgeBits.GetBit(i) > 0 && knowledgeBits.GetBit(i) >= agentKnowledge.GetKnowledgeBit(i))
                {
                    var learning = Math.Min(knowledgeBits.GetBit(i), learningRate * maxRateLearnable);
                    agentKnowledge.Learn(i, learning, step);
                }
            }
        }

        /// <summary>
        ///     Learn a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float Learn(ushort knowledgeId, byte knowledgeBit, float minimumKnowledge, short timeToLive, ushort step)
        {
            if (!LearningModel.On)
            {
                return 0;
            }

            _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return Expertise.GetKnowledge(knowledgeId).Learn(knowledgeBit, NextLearning(), step);
        }

        /// <summary>
        ///     Learn by doing a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="internalCharacteristics"></param>
        /// <param name="step"></param>
        /// <returns>The real learning</returns>
        public float LearnByDoing(ushort knowledgeId, byte knowledgeBit,
            InternalCharacteristics internalCharacteristics, ushort step)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            return LearnByDoing(knowledgeId, knowledgeBit, internalCharacteristics.MinimumRemainingKnowledge,
                internalCharacteristics.TimeToLive, step);
        }

        /// <summary>
        ///     Learn by doing a bit of knowledge
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
            if (!LearningModel.On)
            {
                return 0;
            }

            _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, minimumKnowledge, timeToLive, step);
            return Expertise.GetKnowledge(knowledgeId).Learn(knowledgeBit, NextLearningByDoing(), step);
        }

        public float NextLearning()
        {
            // LearningModel.IsAgentOn is tested at each learning: it is not binary, sometimes you learn, sometimes not
            if (!LearningModel.IsAgentOn())
            {
                return 0;
            }

            var learningStandardDeviation = LearningStandardDeviationValue(LearningStandardDeviation);
            return Normal.Sample(LearningRate, learningStandardDeviation * _randomLevel);
        }

        /// <summary>
        ///     Return the next learning by doing rate
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextLearningByDoing Rate if model is On</returns>
        public float NextLearningByDoing()
        {
            // LearningModel.IsAgentOn is tested at each learning: it is not binary, sometimes you learn, sometimes not
            if (!LearningModel.IsAgentOn())
            {
                return 0;
            }

            var learningStandardDeviation = LearningStandardDeviationValue(LearningStandardDeviation);
            return Normal.Sample(LearningByDoingRate, learningStandardDeviation * _randomLevel);
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

        #endregion

        #region Activities

        /// <summary>
        ///     If set true, agent will be able to perform task if agent has some tasks to perform
        ///     If set to false, agent won't perform any task even if he has some tasks to perform
        /// </summary>
        public bool CanPerformTask { get; set; }

        /// <summary>
        ///     If set true, agent will be able to perform task on weekends if agent has some tasks to perform
        ///     If set to false, agent won't perform any task on weekends even if he has some tasks to perform
        /// </summary>
        public bool CanPerformTaskOnWeekEnds { get; set; }

        /// <summary>
        ///     Get all the activities of an agent
        /// </summary>
        public IEnumerable<string> Activities => _network.NetworkActivities.GetActivities(_id);

        /// <summary>
        ///     Add a list of activities an agent can perform
        /// </summary>
        /// <param name="activities"></param>
        public void AddActivities(IEnumerable<Activity> activities)
        {
            if (!CanPerformTask)
            {
                return;
            }

            _network.NetworkActivities.AddActivities(activities, _id);
        }

        /// <summary>
        ///     List of the activities on which an agent is working, filtered by groupId
        /// </summary>
        public IEnumerable<string> GetGroupActivities(AgentId groupId)
        {
            return _network.NetworkActivities.GetActivities(_id, groupId);
        }

        /// <summary>
        ///     Add all the groupId's activities to the AgentId, filtered by the agentId's knowledges
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="groupId"></param>
        public void AddActivities(AgentId groupId, IEnumerable<string> activities)
        {
            if (!CanPerformTask)
            {
                return;
            }

            _network.NetworkActivities.AddActivities(_id, groupId, activities);
        }

        /// <summary>
        ///     Get the all the knowledges for all the activities of an agent
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, List<Knowledge>>
            GetActivitiesKnowledgesByActivity()
        {
            return _network.NetworkActivities.GetActivitiesKnowledgesByActivity(_id);
        }

        #endregion
    }
}