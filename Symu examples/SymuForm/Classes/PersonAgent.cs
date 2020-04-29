#region Licence

// Description: Symu - SymuForm
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.Templates;
using SymuEngine.Classes.Task;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Common;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Databases.Repository;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace Symu.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = 2;
        private readonly Database _wiki;

        public PersonAgent(ushort agentKey, SymuEngine.Environment.SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(new SimpleHumanTemplate());
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Medium;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            // Communication medium
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email | CommunicationMediums.ViaAPlatform;
            _wiki = Environment.WhitePages.Network.NetworkDatabases.Repository.List.First();
        }

        /// <summary>
        ///     Total tasks done by the agent during the simulation
        /// </summary>
        public ushort WeekTotalTasksDone { get; private set; }

        public AgentId GroupId { get; set; }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        public override void GetNewTasks()
        {
            //Ask a task to the group
            Send(GroupId, MessageAction.Ask, SymuYellowPages.tasks, CommunicationMediums.Email);
        }

        private void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            if (!(e.Task.Parent is Message))
            {
                WeekTotalTasksDone++;
            }

            // The task is about find information about the knowledge
            var knowledgeId = GetKnowledgeId();
            var knowledgeLength = GetKnowledgeLength();
            var bits = new Bits(0);
            bits.InitializeWith0(knowledgeLength);
            // the learning is done randomly on the knowledgeBits
            var index = DiscreteUniform.SampleToByte(0, (byte) (knowledgeLength - 1));
            bits.SetBit(index, 1);
            // Agent is learning
            Cognitive.TasksAndPerformance.Learn(knowledgeId, bits, 1, TimeStep.Step);
            // the complete information is stored in a wiki
            _wiki.StoreKnowledge(knowledgeId, bits, 1, TimeStep.Step);
        }

        private ushort GetKnowledgeId()
        {
            return Environment.WhitePages.Network.NetworkKnowledges.GetKnowledgeIds(Id).First();
        }

        private byte GetKnowledgeLength()
        {
            return Environment.WhitePages.Network.NetworkKnowledges.GetKnowledge(GetKnowledgeId()).Length;
        }

        protected override void ActClassKey(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActClassKey(message);
            switch (message.Subject)
            {
                case SymuYellowPages.tasks:
                    ActTasks(message);
                    break;
            }
        }

        private void ActTasks(Message message)
        {
            if (message.Action != MessageAction.Reply || !message.HasAttachments)
            {
                return;
            }

            // Get a new task
            var task = message.Attachments.First as SymuTask;
            TaskProcessor.Post(task);
        }

        public override void ActEndOfWeek()
        {
            base.ActEndOfWeek();
            var attachments = new MessageAttachments();
            attachments.Add(WeekTotalTasksDone);
            Send(GroupId, MessageAction.Handle, SymuYellowPages.tasks, attachments, CommunicationMediums.Email);
            WeekTotalTasksDone = 0;
        }
    }
}