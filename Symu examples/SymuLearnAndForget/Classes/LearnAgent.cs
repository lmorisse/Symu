#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class LearnAgent : Agent
    {
        public const byte ClassKey = 2;

        public LearnAgent(ushort agentKey, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(agentKey, ClassKey), environment, template)
        {
            Wiki = Environment.WhitePages.Network.NetworkDatabases.Repository.List.First();
            Knowledge = GetKnowledge();
        }

        protected Database Wiki { get; }
        protected Knowledge Knowledge { get; set; }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.ViaAPlatform;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            KnowledgeModel.AddKnowledge(((ExampleEnvironment) Environment).Knowledge.Id,
                ((ExampleEnvironment) Environment).KnowledgeLevel, Cognitive.InternalCharacteristics);
        }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(0)
            {
                Parent = Schedule.Step,
                Weight = 1
            };
            Post(task);
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