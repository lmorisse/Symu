#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace SymuEngine.Messaging.Delayed
{
    /// <summary>
    ///     Manage Delayed MessagesManager
    /// </summary>
    public class DelayedMessages
    {
        /// <summary>
        ///     Key => step
        /// </summary>
        private readonly Dictionary<ushort, Queue<Message.Message>> _messages =
            new Dictionary<ushort, Queue<Message.Message>>();

        public int Count => _messages.Values.Sum(l => l.Count);

        public void Enqueue(Message.Message message, ushort step)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_messages)
            {
                if (!_messages.ContainsKey(step))
                {
                    _messages.Add(step, new Queue<Message.Message>());
                }

                _messages[step].Enqueue(message);
            }
        }

        /// <summary>
        ///     Removes and returns the first Message
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Message.Message Dequeue(ushort step)
        {
            var keys = _messages.Where(m => m.Key <= step && m.Value.Count > 0).OrderBy(m => m.Key).Select(m => m.Key)
                .ToList();
            return keys.Any() ? _messages[keys.First()].Dequeue() : null;
        }

        /// <summary>
        ///     Return the last message of the step without remove it
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Message.Message Last(ushort step)
        {
            var keys = _messages.Where(m => m.Key <= step && m.Value.Count > 0).OrderBy(m => m.Key).Select(m => m.Key)
                .ToList();
            return keys.Any() ? _messages[keys.First()].Last() : null;
        }

        /// <summary>
        ///     Return the last message without remove it
        /// </summary>
        /// <returns></returns>
        public Message.Message Last()
        {
            return _messages.Values.Last().Peek();
        }

        public void Clear()
        {
            _messages.Clear();
        }
    }
}