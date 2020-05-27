#region Licence

// Description: Symu - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Environment;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class LearnByDoingAgent : LearnAgent
    {
        public LearnByDoingAgent(ushort agentKey, SymuEnvironment environment) : base(agentKey, environment)
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
            if (!Environment.Organization.Murphies.IncompleteKnowledge.CheckKnowledge(Knowledge.Id, knowledgeBit, KnowledgeModel.Expertise, Schedule.Step))
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