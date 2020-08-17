#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Common
{
    /// <summary>
    ///     Random generators in order to create random network
    /// </summary>
    public enum RandomGenerator
    {
        /// <summary>
        ///     Linear random in the range [min, max]
        /// </summary>
        RandomUniform,

        /// <summary>
        ///     Random binary values (0 & 1)
        ///     To specify such a network, a cutoff value (called a mean) between zero and one must be speciﬁed;
        ///     this mean specify the mean fraction of ones in the region.
        ///     To ﬁll each value in the region, a random value is chosen from a uniform distribution and its value is compared to
        ///     the mean;
        ///     if the value is less than the mean a one is entered, if it is greater the value is left at zero.
        /// </summary>
        RandomBinary
    }
}