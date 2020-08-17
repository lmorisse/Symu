#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Environment.Events
{
    /// <summary>
    ///     SymuEvent helps you schedule cyclical events that happen during the simulation
    /// </summary>
    public class CyclicalEvent : SymuEvent
    {
        public ushort EveryStep { get; set; }

        public override bool Trigger(ushort step)
        {
            return step % EveryStep == 0;
        }
    }
}