#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Classes.Organization;
using Symu.Common.Interfaces;
using Symu.Engine;
using Symu.Environment;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    public class BaseTestClass
    {
        protected readonly Organization Organization = new Organization("1");
        protected readonly SymuEngine Simulation = new SymuEngine();

        protected readonly SymuEnvironment Environment = new SymuEnvironment();
        protected IEnumerable<IKnowledge> Knowledges => Environment.Organization.MetaNetwork.Knowledge.GetEntities<IKnowledge>();
        protected readonly IAgentId Uid1 = new AgentId(1, 1);
        protected GraphMetaNetwork Network => Organization.MetaNetwork;
        protected WhitePages WhitePages => Environment.WhitePages;
    }
}