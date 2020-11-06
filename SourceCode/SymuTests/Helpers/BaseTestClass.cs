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
        protected SymuEnvironment Environment { get; } = new SymuEnvironment();
        protected MainOrganization MainOrganization { get; } = new MainOrganization("1");
        protected SymuEngine Simulation { get; } = new SymuEngine();
        protected IAgentId Uid1 { get; } = new AgentId(1, 1);

        protected IEnumerable<IKnowledge> Knowledges =>
            Environment.MainOrganization.MetaNetwork.Knowledge.GetEntities<IKnowledge>();

        protected GraphMetaNetwork Network => MainOrganization.MetaNetwork;
        protected AgentNetwork WhitePages => Environment.WhitePages;
    }
}