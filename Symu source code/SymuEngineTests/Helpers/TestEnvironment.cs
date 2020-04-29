#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Environment;

#endregion

namespace SymuEngineTests.Helpers
{
    internal class TestEnvironment : SymuEnvironment
    {
        public virtual void OnBoardAgent(object agent)
        {
        }
    }
}