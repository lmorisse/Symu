#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Repository;

namespace SymuTests.Helpers
{
    internal sealed class TestReactiveAgent : ReactiveAgent
    {
        public static byte ClassId = SymuYellowPages.Actor;
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static TestReactiveAgent CreateInstance(UId id, SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new TestReactiveAgent(id, environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private TestReactiveAgent(UId id, SymuEnvironment environment) : base(new AgentId(id, ClassId), environment)
        {
        }
        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        internal TestReactiveAgent(UId id)
        {
            AgentId = new AgentId(id, ClassId);
        }
        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        internal TestReactiveAgent(byte id)
        {
            AgentId = new AgentId(id, ClassId);

        }
    }
}