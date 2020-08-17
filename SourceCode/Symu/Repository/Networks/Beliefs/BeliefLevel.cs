#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Repository.Networks.Beliefs
{
    /// <summary>
    ///     Measure of a agent's belief on a scale of 0 to 5
    ///     Based on Likert five-level scaling
    /// </summary>
    public enum BeliefLevel
    {
        /// <summary>
        ///     For unit test
        /// </summary>
        NoBelief = 0,
        StronglyDisagree = 1,
        Disagree = 2,
        NeitherAgreeNorDisagree = 3,
        Agree = 4,
        StronglyAgree = 5,
        Random = 6
    }
}