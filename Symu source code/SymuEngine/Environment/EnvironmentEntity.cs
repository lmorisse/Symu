#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Engine;

#endregion

namespace SymuEngine.Environment
{
    public class EnvironmentEntity
    {
        public SimulationRandom RandomLevel { get; set; } = SimulationRandom.NoRandom;
        public byte RandomLevelValue => (byte) RandomLevel;

        public void SetRandomLevel(int level)
        {
            switch (level)
            {
                case 1:
                    RandomLevel = SimulationRandom.Simple;
                    break;
                case 2:
                    RandomLevel = SimulationRandom.Double;
                    break;
                case 3:
                    RandomLevel = SimulationRandom.Triple;
                    break;
                default:
                    RandomLevel = SimulationRandom.NoRandom;
                    break;
            }
        }
    }
}