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
using SymuEngine.Repository.Networks.Knowledge.Agent;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuTools.Classes.ProbabilityDistributions;
using static SymuTools.Classes.Algorithm.Constants;

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
            tasksAndPerformance.CanPerformTask = CanPerformTask;
            LearningModel.CopyTo(tasksAndPerformance.LearningModel);
            TasksLimit.CopyTo(tasksAndPerformance.TasksLimit);
        }

        #region Agent Learning

        public ModelEntity LearningModel { get; set; } = new ModelEntity();

        /// <summary>
        ///     AgentLearningRate define how quickly an agent will learn new knowledge when interacting with other agents
        ///     It impacts the KnowledgeBits of the Agent
        ///     With a rate of 0.01F with no knowledge , an actor become an expert on a specific KnowledgeBit in 100 times
        /// </summary>
        /// <example>
        ///     if AgentLearningRate = 0.01 and the agent is learning on a KnowledgeBit = 0.5 => the upgraded KnowledgeBit =
        ///     0.5 +0.01 = 0.51
        /// </example>
        public float LearningRate { get; set; }

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
        public float LearningByDoingRate { get; set; }

        /// <summary>
        ///     Learning by doing is costly. CostOfLearningByDoing define the factor to know the real cost of the task
        /// </summary>
        /// <example>if a task estimated cost = 1 and CostOfLearningByDoing = 2, then the real task cost = 1*2</example>
        public float CostFactorOfLearningByDoing { get; set; } = 1;

        /// <summary>
        ///     Standard deviation around the LearningRate and LearningByDoingRate
        ///     Default 0.1F
        /// </summary>
        public GenericLevel LearningStandardDeviation { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     Learn by doing a bit of knowledge
        /// </summary>
        /// <param name="knowledgeId">the knowledge Id to learn</param>
        /// <param name="knowledgeBit">the knowledge Bit to learn</param>
        /// <param name="step"></param>
        public void LearnByDoing(ushort knowledgeId, byte knowledgeBit, ushort step)
        {
            if (!LearningModel.IsAgentOn())
            {
                return;
            }

            if (!_network.NetworkKnowledges.Exists(_id, knowledgeId))
            {
                _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, step);
            }

            var workerKnowledge = Expertise.GetKnowledge(knowledgeId);
            workerKnowledge.Learn(knowledgeBit, NextLearningByDoing(), step);
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
            if (knowledgeId == 0 || knowledgeBits == null || Math.Abs(LearningRate) < tolerance ||
                Math.Abs(maxRateLearnable) < tolerance)
            {
                return;
            }

            if (knowledgeId > 0 && knowledgeBits == null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, step);
            var workerKnowledge = Expertise.GetKnowledge(knowledgeId);
            Learn(knowledgeBits, maxRateLearnable, workerKnowledge, step);
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
            if (Math.Abs(learningRate * maxRateLearnable) < tolerance)
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
        /// <param name="step"></param>
        public void Learn(ushort knowledgeId, byte knowledgeBit, ushort step)
        {
            if (!LearningModel.IsAgentOn())
            {
                return;
            }

            _network.NetworkKnowledges.LearnNewKnowledge(_id, knowledgeId, step);
            Expertise.GetKnowledge(knowledgeId).Learn(knowledgeBit, NextLearning(), step);
        }

        public float NextLearning()
        {
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

        #endregion

        #region Activities

        public bool CanPerformTask { get; set; }

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
        public IDictionary<string, List<Repository.Networks.Knowledge.Repository.Knowledge>>
            GetActivitiesKnowledgesByActivity()
        {
            return _network.NetworkActivities.GetActivitiesKnowledgesByActivity(_id);
        }

        #endregion
    }
}