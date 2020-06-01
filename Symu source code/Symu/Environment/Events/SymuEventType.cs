#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Environment.Events
{
    /// <summary>
    ///     Type of SymuEvent used to schedule event during the simulation
    /// </summary>
    public enum SymuEventType
    {
        NoEvent = 0,
        OneShot = 1,
        Cyclical = 2,
        Random = 3
    }
}