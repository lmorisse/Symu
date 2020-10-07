#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal sealed class TestCognitiveAgent : CognitiveAgent
    {
        public static byte Class = SymuYellowPages.Actor;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private TestCognitiveAgent(IAgentId id, SymuEnvironment environment) : base(id, environment,
            environment.MainOrganization.Templates.Human)
        {
        }

        public static IClassId ClassId => new ClassId(Class);

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static TestCognitiveAgent CreateInstance(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new TestCognitiveAgent(environment.WhitePages.NextAgentId(Class), environment);
            agent.Initialize();
            return agent;
        }

        public static TestCognitiveAgent CreateInstance(byte classId, SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new TestCognitiveAgent(environment.WhitePages.NextAgentId(classId), environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     Initialize the agent models
        /// </summary>
        /// <remarks>
        ///     Should be called after the constructor
        ///     Use the factory method CreateInstance instead of the constructor to call Initialize implicitly
        /// </remarks>
        public override ReactiveAgent Clone()
        {
            var agent = (TestCognitiveAgent) base.Clone();
            agent.Initialize();
            return agent;
        }
    }
}