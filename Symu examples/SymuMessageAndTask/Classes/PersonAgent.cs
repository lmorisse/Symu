#region Licence

// Description: Symu - SymuMessageAndTask
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Task;
using SymuEngine.Classes.Task.Manager;
using SymuEngine.Environment;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository;

#endregion

namespace SymuMessageAndTask.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = 2;

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.SimpleHuman);
            // Communication medium
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
        }

        public AgentId GroupId { get; set; }

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

        protected override void ActClassKey(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActClassKey(message);
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
            if (message.Attachments.First is List<SymuTask> tasks)
            {
                foreach (var task in tasks)
                {
                    TaskProcessor.Post(task);
                }
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