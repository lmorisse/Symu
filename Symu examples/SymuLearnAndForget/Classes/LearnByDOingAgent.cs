#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class LearnByDoingAgent : LearnAgent
    {

        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static LearnByDoingAgent CreateInstance(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            var agent = new LearnByDoingAgent(id, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private LearnByDoingAgent(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(id, environment, template)
        {
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(0)
            {
                Parent = Schedule.Step,
                // Cost impact of learning by doing
                Weight = 1 * Environment.Organization.Murphies.IncompleteKnowledge.CostFactorOfGuessing
            };
            Post(task);
        }

        /// <summary>
        ///     each task allows to learn by doing a little more about knowledge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            // the learning is done randomly on the one knowledgeBit
            var knowledgeBit = Knowledge.GetRandomBitIndex();

            // Agent don't know enough to do it (and learn it) by himself
            // He needs minimum initial knowledge to do and learn
            if (!Environment.Organization.Murphies.IncompleteKnowledge.CheckKnowledge(Knowledge.Id, knowledgeBit,
                KnowledgeModel, Schedule.Step))
            {
                return;
            }

            // Agent is learning
            var realLearning = LearningModel.LearnByDoing(Knowledge.Id, knowledgeBit, Schedule.Step);
            // the knowledge is stored in a wiki
            Wiki.StoreKnowledge(Knowledge.Id, knowledgeBit, realLearning, Schedule.Step);
        }
    }
}