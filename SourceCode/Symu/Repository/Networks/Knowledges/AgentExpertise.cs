#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using static Symu.Common.Constants;

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Expertise of an agent is defined by the list of all its knowledge (hard skills)  x KnowledgeLevel
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class AgentExpertise
    {
        /// <summary>
        ///     Key => ComponentId
        ///     Values => List of Knowledge
        /// </summary>
        public List<AgentKnowledge> List { get; } = new List<AgentKnowledge>();

        /// <summary>
        ///     Accumulates all learning of the agent for all knowledge during the simulation
        /// </summary>
        public float CumulativeLearning => List.Any() ? List.Sum(t => t.CumulativeLearning) : 0;

        /// <summary>
        ///     Accumulates all forgetting of the agent for all knowledge during the simulation
        /// </summary>
        public float CumulativeForgetting => List.Any() ? List.Sum(t => t.CumulativeForgetting) : 0;

        /// <summary>
        ///     Percentage of all forgetting of the agent for all knowledge during the simulation
        /// </summary>
        public float PercentageForgetting
        {
            get
            {
                float percentage = 0;
                var sum = CumulativeForgetting;
                var sumKnowledge = GetKnowledgeSum();
                if (sumKnowledge > Tolerance)
                {
                    percentage = 100 * sum / sumKnowledge;
                }

                return percentage;
            }
        }

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
        ///     Percentage of all Knowledge of the agent for all knowledge during the simulation
        /// </summary>
        public float PercentageKnowledge
        {
            get
            {
                float percentage = 0;
                var sum = GetKnowledgeSum();
                var potential = GetKnowledgePotential();
                if (potential > Tolerance)
                {
                    percentage = 100 * sum / potential;
                }

                return percentage;
            }
        }

        public int Count => List.Count;

        /// <summary>
        ///     Average of all the knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public float Obsolescence(float step)
        {
            return List.Any() ? List.Average(t => t.Obsolescence(step)) : 0;
        }

        /// <summary>
        ///     EventHandler triggered after learning a new information
        /// </summary>
        public event EventHandler<LearningEventArgs> OnAfterLearning;

        /// <summary>
        ///     Get the sum of all the knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgeSum()
        {
            return List.Sum(l => l.GetKnowledgeSum());
        }

        /// <summary>
        ///     Get the maximum potential knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgePotential()
        {
            return List.Sum(l => l.GetKnowledgePotential());
        }

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(AgentKnowledge agentKnowledge)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (Contains(agentKnowledge))
            {
                return;
            }

            agentKnowledge.OnAfterLearning += AfterLearning;
            List.Add(agentKnowledge);
        }

        public void Add(ushort knowledgeId, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            var agentKnowledge = new AgentKnowledge(knowledgeId, level, minimumKnowledge, timeToLive);
            Add(agentKnowledge);
        }

        public bool Contains(AgentKnowledge agentKnowledge)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            return Contains(agentKnowledge.KnowledgeId);
        }

        public bool Contains(ushort knowledgeId)
        {
            return List.Exists(x => x.KnowledgeId == knowledgeId);
        }

        public AgentKnowledge GetKnowledge(ushort knowledgeId)
        {
            return List.Find(x => x.KnowledgeId == knowledgeId);
        }

        /// <summary>
        ///     Get all the knowledge of an agent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ushort> GetKnowledgeIds()
        {
            return List.Select(x => x.KnowledgeId);
        }

        /// <summary>
        ///     Check that agent has the knowledgeId[knowledgeBit] == 1
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if the agent has the knowledge</returns>
        public bool KnowsEnough(ushort knowledgeId, byte knowledgeBit, float knowledgeThreshHoldForAnswer, ushort step)
        {
            if (!Contains(knowledgeId))
            {
                return false;
            }

            var knowledge = GetKnowledge(knowledgeId);
            return knowledge.KnowsEnough(knowledgeBit, knowledgeThreshHoldForAnswer, step);
        }

        /// <summary>
        ///     Forget knowledges from the expertise based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public void ForgettingProcess(float forgettingRate, ushort step)
        {
            List.ForEach(x => x.ForgettingProcess(forgettingRate, step));
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