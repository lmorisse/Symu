#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Classes.Murphies
{
    /// <summary>
    ///     Base Class to model a murphy
    ///     We design an optimal operating system
    ///     But under internal stress conditions, we have a suboptimal operating system
    ///     A murphy is an event that create a stress condition
    ///     It's a murphy when the stress has an internal source
    ///     It's a mayday when the stress has an external source
    /// </summary>
    /// <example>
    ///     Time pressure
    ///     Incomplete/changing/incorrect information
    ///     Communication breakdown
    ///     agent unavailable (bottleneck, illness, holidays, ...)
    /// </example>
    public abstract class Murphy
    {
        /// <summary>
        ///     If (On) the Murphy is active
        /// </summary>
        public bool On { get; set; } = false;
    }
}