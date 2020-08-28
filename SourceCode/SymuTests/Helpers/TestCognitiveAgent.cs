#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
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
        public static ClassId ClassId => new ClassId(Class);
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static TestCognitiveAgent CreateInstance(UId id, SymuEnvironment environment)
        {
            var agent = new TestCognitiveAgent(id, environment);
            agent.Initialize();
            return agent;
        }
        public static TestCognitiveAgent CreateInstance(UId id, byte classId, SymuEnvironment environment)
        {
            var agent = new TestCognitiveAgent(id, classId, environment);
            agent.Initialize();
            return agent;
        }
        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private TestCognitiveAgent(UId id, SymuEnvironment environment) : base(new AgentId(id, Class), environment,
            environment.Organization.Templates.Human)
        {
        }

        private TestCognitiveAgent(UId id, byte classId, SymuEnvironment environment) : base(new AgentId(id, classId),
            environment, environment.Organization.Templates.Human)
        {
        }
    }
}