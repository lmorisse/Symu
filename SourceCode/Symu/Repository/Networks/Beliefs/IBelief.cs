#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces.Entity;

namespace Symu.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Describe a belief, based on knowledge/fact
    /// </summary>
    public interface IBelief
    {
        /// <summary>
        ///     Unique identifier af the knowledge
        /// </summary>
        IId Id { get; }        
        /// <summary>
        ///     Each area of knowledge is represented by a collection of KnowledgeBits
        ///     The size define the length of the collection
        ///     each bit represent a single atomic fact
        ///     size range [0; 10]
        /// </summary>
        byte Length { get; }
        bool Equals(IBelief belief);
    }
}