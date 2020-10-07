#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion


using Symu.Common.Interfaces;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     SymuEvent helps you schedule cyclical events that happen during the simulation
    /// </summary>
    public class CyclicalEvent : EventEntity
    {
        public ushort EveryStep { get; set; }

        public override bool Trigger(ushort step)
        {
            return step % EveryStep == 0;
        }
        public CyclicalEvent(){}
        public CyclicalEvent(GraphMetaNetwork metaNetwork) : base(metaNetwork)
        {
        }
        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new CyclicalEvent();
            CopyEntityTo(clone);
            return clone;
        }

        public override void CopyEntityTo(IEntity entity)
        {
            base.CopyEntityTo(entity);
            if (!(entity is CyclicalEvent copy))
            {
                return;
            }
            copy.EveryStep = EveryStep;
        }
    }
}