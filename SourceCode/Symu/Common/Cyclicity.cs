#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Common
{
    /// <summary>
    ///     Cyclicity used in Symu
    ///     like SymuEvent : to schedule event during the simulation
    /// </summary>
    public enum Cyclicity
    {
        None = 0,
        OneShot = 1,
        Cyclical = 2,
        Random = 3,
        Always
    }
}