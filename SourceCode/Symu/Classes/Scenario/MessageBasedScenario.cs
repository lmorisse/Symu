#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Environment;

#endregion

namespace Symu.Classes.Scenario
{
    public class MessageBasedScenario : SimulationScenario
    {/// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static MessageBasedScenario CreateInstance(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new MessageBasedScenario(environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private MessageBasedScenario(SymuEnvironment environment) : base(null, environment)
        {
        }

        /// <summary>
        ///     Number of steps max to process
        ///     In the same unit of TimeStepType
        ///     Use NoLimit for a scenario with no end
        /// </summary>
        public ushort NumberOfMessages { get; set; }

        public static sbyte NoLimit { get; } = -1;

        public override void PreStep()
        {
            base.PreStep();
            if (NumberOfMessages != NoLimit && Environment.Messages.Result.SentMessagesCount >= NumberOfMessages - 1)
            {
                State = AgentState.Stopping;
            }
        }

        public override SimulationScenario Clone()
        {
            var clone = new MessageBasedScenario(Environment)
            {
                NumberOfMessages = NumberOfMessages
            };
            return clone;
        }
    }
}