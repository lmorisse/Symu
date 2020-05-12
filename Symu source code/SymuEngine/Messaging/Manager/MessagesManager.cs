#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SymuEngine.Messaging.Messages;

#endregion

namespace SymuEngine.Messaging.Manager
{
    internal class MessagesManager : IDisposable
    {
        private readonly Queue<Message> _arrivals;

        private TaskCompletionSource<bool> _savedCont;

        public MessagesManager()
        {
            _arrivals = new Queue<Message>();
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        private async Task<bool> WaitOneNoTimeout()
        {
            if (_savedCont != null)
            {
                throw new Exception("multiple waiting reader continuations for mailbox");
            }

            bool descheduled;
            // An arrival may have happened while we're preparing to deschedule
            lock (_arrivals)
            {
                if (_arrivals.Count == 0)
                {
                    _savedCont = new TaskCompletionSource<bool>();
                    descheduled = true;
                }
                else
                {
                    descheduled = false;
                }
            }

            if (!descheduled)
            {
                return true;
            }

            return await _savedCont.Task.ConfigureAwait(false);
        }

        internal Message ReceiveFromArrivals()
        {
            lock (_arrivals)
            {
                return _arrivals.Count == 0 ? default : _arrivals.Dequeue();
            }
        }

        internal void Post(Message msg)
        {
            lock (_arrivals)
            {
                _arrivals.Enqueue(msg);

                // This is called when we enqueue a message, within a lock
                // We cooperatively unblock any waiting reader. If there is no waiting
                // reader we just leave the message in the incoming queue

                if (_savedCont == null)
                {
                    return;
                }

                var sc = _savedCont;
                _savedCont = null;
                sc.SetResult(true);
            }
        }

        internal async Task<Message> Receive()
        {
            async Task<Message> ProcessFirstArrival()
            {
                while (true)
                {
                    var res = ReceiveFromArrivals();
                    if (res != null)
                    {
                        return res;
                    }

                    var ok = await WaitOneNoTimeout().ConfigureAwait(false);
                    if (ok)
                    {
                        continue;
                    }

                    throw new TimeoutException("Mailbox Receive Timed Out");
                }
            }

            return await ProcessFirstArrival().ConfigureAwait(false);
        }
    }
}