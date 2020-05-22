#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal sealed class TestAgent : Agent
    {
        public static byte ClassKey = SymuYellowPages.Actor;

        public TestAgent(ushort key, SymuEnvironment environment) : base(new AgentId(key, ClassKey), environment)
        {
            SetCognitive(new SimpleHumanTemplate());
        }

        public TestAgent(ushort key, byte classKey, SymuEnvironment environment) : base(new AgentId(key, classKey),
            environment)
        {
            SetCognitive(new SimpleHumanTemplate());
        }
    }
}