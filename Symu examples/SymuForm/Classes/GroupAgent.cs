#region Licence

// Description: Symu - SymuForm
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.Templates;
using SymuEngine.Classes.Task;
using SymuEngine.Messaging.Message;
using SymuEngine.Repository;

#endregion

namespace Symu.Classes
{
    public sealed class GroupAgent : Agent
    {
        public const byte ClassKey = 1;

        public GroupAgent(ushort agentKey, SymuEngine.Environment.SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(new StandardAgentTemplate());
        }

        /// <summary>
        ///     Total tasks done by the agent during the simulation
        /// </summary>
        public ushort TotalTasksDone { get; private set; }

        public override void ActMessage(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            base.ActMessage(message);
            switch (message.Subject)
            {
                case SymuYellowPages.tasks:
                    ActTasks(message);
                    break;
            }
        }

        private void ActTasks(Message message)
        {
            switch (message.Action)
            {
                case MessageAction.Ask:
                    // Create the next task 
                    var task = new SymuTask(0)
                    {
                        Parent = TimeStep.Step,
                        Weight = 1
                    };
                    // Send it to the sender
                    var reply = Message.ReplyMessage(message);
                    reply.Attachments = new MessageAttachments();
                    reply.Attachments.Add(task);
                    Send(reply);
                    break;
                case MessageAction.Handle:
                    TotalTasksDone += (ushort) message.Attachments.First;
                    break;
            }
        }
    }
}