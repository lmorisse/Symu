#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Messaging.Messages;

#endregion

namespace Symu.Messaging.Tracker
{
    /// <summary>
    ///     Manage TimeStamped MessagesManager
    /// </summary>
    public class TimeStampedMessages
    {
        /// <summary>
        ///     Key => step
        /// </summary>
        private readonly Dictionary<ushort, List<Message>> _messages =
            new Dictionary<ushort, List<Message>>();

        public ushort MinStep => _messages.Any() ? _messages.Keys.Min() : (ushort) 0;

        public ushort MaxStep => _messages.Any() ? _messages.Keys.Max() : (ushort) 0;

        public int Count => _messages.Values.Sum(l => l.Count);
        public bool Any => Count > 0;

        public IEnumerable<byte> Contents => _messages.Values.SelectMany(l => l).Select(l => l.Subject).Distinct();

        public void Enqueue(Message message, ushort step)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_messages)
            {
                if (!_messages.ContainsKey(step))
                {
                    _messages.Add(step, new List<Message>());
                }

                _messages[step].Add(message);
            }
        }

        /// <summary>
        ///     Initialize all MessagesManager before stepNumber
        /// </summary>
        /// <param name="stepNumber">Don't cast to ushort to avoid problem with negative values</param>
        public void ClearSteps(int stepNumber)
        {
            foreach (var step in _messages.Keys.ToList().FindAll(k => k <= stepNumber))
            {
                _messages.Remove(step);
            }
        }

        /// <summary>
        ///     Initialize all MessagesManager
        /// </summary>
        public void Clear()
        {
            _messages.Clear();
        }


        public List<Message> ByContent(ushort step, byte content)
        {
            return _messages.ContainsKey(step) ? _messages[step].ToList().FindAll(m => m.Subject == content) : null;
        }

        public List<Message> ReceivedByAgent(ushort step, IAgentId agentId)
        {
            return _messages.ContainsKey(step)
                ? _messages[step].ToList().FindAll(m => m.Receiver.Equals(agentId))
                : null;
        }

        public List<Message> SentByAgent(ushort step, IAgentId agentId)
        {
            return _messages.ContainsKey(step) ? _messages[step].ToList().FindAll(m => m.Sender.Equals(agentId)) : null;
        }

        public bool Exists(MessageAction action, byte content, IClassId senderClassId, IClassId receiverClassId)
        {
            return _messages.Any(m => m.Value.Exists(v => v.Action == action && v.Subject == content
                                                                             && v.Sender.Equals(senderClassId) &&
                                                                             v.Receiver.Equals(receiverClassId)));
        }

        public bool Exists(MessageAction action, byte content, IClassId senderClassId)
        {
            return _messages.Any(m => m.Value.Exists(v => v.Action == action && v.Subject == content
                                                                             && v.Sender.Equals(senderClassId)));
        }
    }
}