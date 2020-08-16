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
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace SymuMessageAndTask.Classes
{
    public sealed class GroupAgent : ReactiveAgent
    {
        public const byte ClassKey = 1;

        public GroupAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey), environment)
        {
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
            switch (message.Action)
            {
                case MessageAction.Ask:
                    AskTask(message);
                    break;
            }
        }

        private void AskTask(Message message)
        {
            if (!(Environment is ExampleEnvironment environment))
            {
                return;
            }

            var tasks = new List<SymuTask>();
            for (var i = 0; i < environment.NumberOfTasks; i++)
            {
                // Create the next task 
                var task = new SymuTask(Schedule.Step)
                {
                    Parent = Schedule.Step,
                    Weight = environment.CostOfTask
                };
                tasks.Add(task);
            }

            // Send it to the sender
            var reply = Message.ReplyMessage(message);
            reply.Attachments = new MessageAttachments();
            reply.Attachments.Add(tasks);
            Send(reply);
        }
    }
}