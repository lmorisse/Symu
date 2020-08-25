#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Repository.Networks.Roles
{
    /// <summary>
    /// Defines the role of an agent for the RoleNetwork
    /// </summary>
    public interface IRole
    {
        bool Equals(IRole role);
    }
}