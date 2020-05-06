#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent;
using SymuEngine.Repository;

#endregion

namespace SymuEngine.Classes.Scenario
{
    public class ScenarioEntity : AgentEntity
    {
        public const byte ClassKey = SymuYellowPages.Scenario;

        public ScenarioEntity(ushort key) : base(key, ClassKey)
        {
        }
    }
}