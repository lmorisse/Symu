#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using System;
using Symu.DNA.OneModeNetworks.Resource;

namespace Symu.Repository.Entity
{
    /// <summary>
    /// Default implementation of IResourceUsage
    /// </summary>
    public class ResourceUsage : IResourceUsage
    {
        public ResourceUsage(byte usage)
        {
            Usage = usage;
        }
        public byte Usage { get; }
        public bool Equals(IResourceUsage resourceUsage)
        {
            if (resourceUsage == null)
            {
                throw new ArgumentNullException(nameof(resourceUsage));
            }

            return resourceUsage is ResourceUsage usage &&
                   Usage == usage.Usage;
        }
    }
}