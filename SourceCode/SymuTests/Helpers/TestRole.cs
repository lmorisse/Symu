#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Repository.Networks.Roles;

namespace SymuTests.Helpers
{
    internal sealed class TestRole : IRole
    {
        public TestRole(byte role)
        {
            Role = role;
        }
        public readonly byte Role;
        public bool Equals(IRole role)
        {
            return role is TestRole rol
                   && Role == rol.Role;
        }
    }
}