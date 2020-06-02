#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    /// SplitStep is a special class which allow an agent to split a step into a number of steps
    /// using messages between agent and its environment
    /// </summary>
    public class SplitStep
    {
        private readonly AgentId _agentId;
        private readonly SymuEnvironment _environment;
        private const byte NumberOfSplits = 10;
        public float ActualRatio => (float)_actualSplit / NumberOfSplits;
        private byte _actualSplit;
        /// <summary>
        ///     EventHandler triggered after the message is received by agent to act during the actual split and call the next split
        ///     This event is triggered in the Agent.Act() method
        /// </summary>
        public event EventHandler OnStep;


        public SplitStep(SymuEnvironment environment, AgentId agentId)
        {
            _environment = environment;
            _agentId = agentId;
        }

        /// <summary>
        /// If NumberOfSplits is reached, 
        /// </summary>
        /// <returns>false if NumberOfSplits is reached</returns>
        /// <returns>true if message is send to agent</returns>
        public bool NextSplit()
        {
            if (_actualSplit >= NumberOfSplits)
            {
                return false;
            }
            _actualSplit++;
            var message = new Message(_agentId, _agentId, MessageAction.Handle, SymuYellowPages.SplitStep, this, CommunicationMediums.System);
            _environment.SendAgent(message);
            return true;
        }

        public void Step()
        {
            OnStep?.Invoke(this, null);
        }
    }
}