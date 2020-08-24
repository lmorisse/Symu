#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public sealed class GroupAgent : ReactiveAgent
    {
        public const byte Class = 1;

        public GroupAgent(UId id, SymuEnvironment environment) : base(
            new AgentId(id, Class), environment)
        {
        }
    }
}