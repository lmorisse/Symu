#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Classes.Agents;
using Symu.Environment;
using Symu.Repository;

namespace SymuTests.Helpers
{
    internal sealed class TestReactiveAgent : ReactiveAgent
    {
        public static byte ClassKey = SymuYellowPages.Actor;

        public TestReactiveAgent(ushort key, SymuEnvironment environment) : base(new AgentId(key, ClassKey), environment)
        {
        }

        public TestReactiveAgent(ushort key, byte classKey, SymuEnvironment environment) : base(new AgentId(key, classKey), environment)
        {
        }
    }
}