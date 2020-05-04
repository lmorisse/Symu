#region Licence

// Description: Symu - SymuLearnAndForget
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Task;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class LearnAgent : Agent
    {
        public const byte ClassKey = 2;

        public LearnAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.OrganizationEntity.Templates.SimpleHuman);
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.ViaAPlatform;
            Wiki = Environment.WhitePages.Network.NetworkDatabases.Repository.List.First();
            Knowledge = GetKnowledge();
        }

        protected Database Wiki { get; }
        protected Knowledge Knowledge { get; set; }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(0)
            {
                Parent = TimeStep.Step,
                Weight = 1
            };
            TaskProcessor.Post(task);
        }

        /// <summary>
        ///     each task allows to learn a little more about knowledge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
        }

        private Knowledge GetKnowledge()
        {
            var knowledgeId = Environment.WhitePages.Network.NetworkKnowledges.Repository.List.First().Id;
            return Environment.WhitePages.Network.NetworkKnowledges.GetKnowledge(knowledgeId);
        }
    }
}