#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Repository.Networks.Resources
{
    /// <summary>
    /// Define the type of usage done of a resource,
    /// if you want to specify several usages that can be done: using, working on, borrowing, ...
    /// </summary>
    public interface IResourceUsage
    {
        /// <summary>
        /// Does the agent using the resource in a specific way
        /// </summary>
        /// <param name="resourceUsage"></param>
        /// <returns></returns>
        bool IsResourceUsage(IResourceUsage resourceUsage);
    }
}