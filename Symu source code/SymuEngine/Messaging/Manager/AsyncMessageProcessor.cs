#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Threading.Tasks;

#endregion

namespace Symu.Messaging.Manager
{
    public static class AsyncMessageProcessor
    {
        public static MessageProcessor Start(Func<MessageProcessor, Task> body)
        {
            var messageProcessor = new MessageProcessor(body);
            messageProcessor.Start();
            return messageProcessor;
        }
    }
}