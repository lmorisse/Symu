#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Measure of a employee's expertise on a scale of 0 to 5
    /// </summary>
    public enum KnowledgeLevel
    {
        NoKnowledge = 0,
        BasicKnowledge = 1,
        Foundational = 2,
        Intermediate = 3,

        /// <summary>
        ///     Able to work independently on routine items
        /// </summary>
        FullProficiency = 4,

        /// <summary>
        ///     Able to train and coach others
        /// </summary>
        Expert = 5,

        /// <summary>
        ///     For unit test
        /// </summary>
        FullKnowledge = 6,

        Random = 7
    }
}