#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Task;
using SymuEngine.Common;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     Tasks & Density from Construct Software
    ///     Activities
    ///     MultiTasking
    ///     Performs tasks
    ///     learning by doing
    /// </summary>
    /// <remarks>Tasks and Density from Construct Software</remarks>
    public class TasksAndPerformance
    {
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
            TasksLimit.CopyTo(tasksAndPerformance.TasksLimit);
        }

        #region Agent Learning


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

        #endregion
    }
}