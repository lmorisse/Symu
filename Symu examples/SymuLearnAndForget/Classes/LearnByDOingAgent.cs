#region Licence

// Description: Symu - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Task;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Environment;

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
                Parent = TimeStep.Step,
                // Cost impact of learning by doing
                Weight = 1 * Cognitive.TasksAndPerformance.CostFactorOfLearningByDoing
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
            if (!KnowledgeModel.CheckKnowledge(Knowledge.Id, knowledgeBit, TimeStep.Step))
            {
                return;
            }

            // Agent is learning
            var realLearning = LearningModel.LearnByDoing(Knowledge.Id, knowledgeBit, TimeStep.Step);
            // the knowledge is stored in a wiki
            Wiki.StoreKnowledge(Knowledge.Id, knowledgeBit, realLearning, TimeStep.Step);
        }
    }
}