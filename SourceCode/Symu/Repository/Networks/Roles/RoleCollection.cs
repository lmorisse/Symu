#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Networks.Resources;

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
        ///     Key => DatabaseId
        ///     Values => List of Databases
        /// </summary>
        public List<IResource> List { get; } = new List<IResource>();

        public void Add(IResource resource)
        {
            if (!Contains(resource))
            {
                List.Add(resource);
            }
        }

        public bool Contains(IResource resource)
        {
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return Exists(resource.Id);
        }

        public IResource Get(IId roleId)
        {
            return List.Find(k => k.Id.Equals(roleId));
        }

        public bool Exists(IId roleId)
        {
            return List.Exists(k => k.Id.Equals(roleId));
        }

        public void Clear()
        {
            List.Clear();
        }
    }
}