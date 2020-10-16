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
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace SymuExamples.MessageAndTask
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = 2;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            ClassId, environment, template)
        {
        }

        public static IClassId ClassId => new ClassId(Class);
        private ExampleMainOrganization MainOrganization => ((ExampleEnvironment) Environment).ExampleMainOrganization;

        public IAgentId GroupId { get; set; }

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new PersonAgent(environment, template);
            agent.Initialize();
            return agent;
        }

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
            Send(GroupId, MessageAction.Ask, SymuYellowPages.Task, CommunicationMediums.Email);
        }

        private void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            if (!(e.Task.Parent is Message))
            {
                // warns the group that he has performed the task
                Send(GroupId, MessageAction.Close, SymuYellowPages.Task, CommunicationMediums.Email);
            }
        }

        /// <summary>
        ///     Use to set the baseline value of the initial capacity
        /// </summary>
        /// <returns></returns>
        public override void SetInitialCapacity()
        {
            Capacity.Initial = MainOrganization.InitialCapacity;
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
                case SymuYellowPages.Task:
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
            var switchingContextCost = MainOrganization.SwitchingContextCost;
            Capacity.Multiply(1 / switchingContextCost);
        }
    }
}