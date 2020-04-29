#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Results.Organization
{
    /// <summary>
    ///     One of the most fundamental types of groups is the triads
    ///     Rapid formation and reformation of triads is one key aspect of flexibility
    /// </summary>
    public readonly struct TriadsStruct
    {
        public TriadsStruct(uint numberOfTriads, uint maxTriads, ushort step)
        {
            NumberOfTriads = numberOfTriads;
            MaxTriads = maxTriads;
            Step = step;
        }

        public uint MaxTriads { get; }
        public uint NumberOfTriads { get; }
        public ushort Step { get; }

        /// <summary>
        ///     Percentage of the theoretical value
        /// </summary>
        public float Performance => NumberOfTriads * 100F / MaxTriads;

        public override string ToString()
        {
            return "Performance " + Performance + " / step" + Step;
        }
    }
}