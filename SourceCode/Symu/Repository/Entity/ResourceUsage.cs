#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using System;
using Symu.Repository.Networks.Resources;

namespace Symu.Repository.Entity
{
    /// <summary>
    /// Implement IResourceUsage
    /// </summary>
    public class ResourceUsage : IResourceUsage
    {
        public ResourceUsage(byte usage)
        {
            Usage = usage;
        }
        public byte Usage { get; }
        public bool IsResourceUsage(IResourceUsage resourceUsage)
        {
            if (resourceUsage == null)
            {
                throw new ArgumentNullException(nameof(resourceUsage));
            }

            return Usage == ((ResourceUsage)resourceUsage).Usage;
        }
    }
}