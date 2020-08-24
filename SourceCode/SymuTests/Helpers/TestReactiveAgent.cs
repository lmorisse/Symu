#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

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

        public TestReactiveAgent(UId id, SymuEnvironment environment) : base(new AgentId(id, ClassId), environment)
        {
        }

        public TestReactiveAgent(UId id, byte classId, SymuEnvironment environment) : base(new AgentId(id, classId), environment)
        {
        }
    }
}