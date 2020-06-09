#region Licence

// Description: SymuBiz - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace SymuMessageAndTask.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = 2;

        public PersonAgent(ushort agentKey, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(agentKey, ClassKey), environment, template)
        {
        }

        public AgentId GroupId { get; set; }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            // Communication medium
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.Random;
        }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        public override void GetNewTasks()
        {
            //Ask a task to the group
            Send(GroupId, MessageAction.Ask, SymuYellowPages.Tasks, CommunicationMediums.Email);
        }

        private void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            if (!(e.Task.Parent is Message))
            {
                // warns the group that he has performed the task
                Send(GroupId, MessageAction.Close, SymuYellowPages.Tasks, CommunicationMediums.Email);
            }
        }

        /// <summary>
        ///     Use to set the baseline value of the initial capacity
        /// </summary>
        /// <returns></returns>
        public override void SetInitialCapacity()
        {
            Capacity.Initial = ((ExampleEnvironment) Environment).InitialCapacity;
        }

        public override void ActMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActMessage(message);
            switch (message.Subject)
            {
                case SymuYellowPages.Tasks:
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

            // Get new tasks
            if (!(message.Attachments.First is List<SymuTask> tasks))
            {
                return;
            }

            foreach (var task in tasks)
            {
                Post(task);
            }
        }

        public override void ActEndOfWeek()
        {
            base.ActEndOfWeek();
            // warns the group that he is leaving for the weekend
            Send(GroupId, MessageAction.Stop, SymuYellowPages.EndOfWeek, CommunicationMediums.Email);
        }

        public override void ActEndOfDay()
        {
            base.ActEndOfDay();
            // warns the group that he has finished his day
            Send(GroupId, MessageAction.Stop, SymuYellowPages.WorkingDay, CommunicationMediums.Email);
        }

        public override void SwitchingContextModel()
        {
            var switchingContextCost = ((ExampleEnvironment) Environment).SwitchingContextCost;
            Capacity.Multiply(1 / switchingContextCost);
        }
    }
}