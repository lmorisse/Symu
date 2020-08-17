#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Common.Interfaces
{
    /// <summary>
    /// IClassKey is the interface for the unique identifier of the class of the agent
    /// </summary>
    public interface IClassId
    {
        bool Equals(IClassId classId);
    }
}