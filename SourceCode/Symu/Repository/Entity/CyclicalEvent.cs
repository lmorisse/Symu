#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces.Entity;

namespace Symu.Repository.Entity
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
        public CyclicalEvent(ushort id) : base(id)
        {
        }
        public CyclicalEvent(IId id) : base(id)
        {
        }
    }
}