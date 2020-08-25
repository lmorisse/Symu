#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace Symu.Repository.Networks.Roles
{
    /// <summary>
    ///     List of all the roles 
    ///     Used by roleNetwork
    /// </summary>
    public class RoleCollection
    {
        /// <summary>
        ///     List of all the roles used in the network
        /// </summary>
        public List<IRole> List { get; } = new List<IRole>();

        public void Add(IRole role)
        {
            if (!Contains(role))
            {
                List.Add(role);
            }
        }

        public bool Contains(IRole role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return List.Contains(role);
        }

        public void Clear()
        {
            List.Clear();
        }
    }
}