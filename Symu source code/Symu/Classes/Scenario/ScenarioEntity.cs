#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Repository;

#endregion

namespace Symu.Classes.Scenario
{
    public class ScenarioEntity : AgentEntity
    {
        public const byte Class = SymuYellowPages.Scenario;

        public ScenarioEntity(ushort key) : base(key, Class)
        {
        }
    }
}