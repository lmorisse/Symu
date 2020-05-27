#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Engine
{
    public enum RandomLevel
    {
        NoRandom = 0,
        Simple = 1,
#pragma warning disable CA1720 // L'identificateur contient le nom de type
        Double = 2,
#pragma warning restore CA1720 // L'identificateur contient le nom de type
        Triple = 3
    }
}