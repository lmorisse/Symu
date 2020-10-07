#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

#endregion

#region using directives

using System;
using Symu.Common.Interfaces;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     EventEntity helps you schedule one shot events that happen during the simulation
    /// </summary>
    public class EventEntity: OrgMod.Entities.EventEntity
    {
        public EventEntity() 
        {
        }
        public EventEntity(GraphMetaNetwork metaNetwork) : base(metaNetwork)
        {
        }

        public ushort Step { get; set; }

        /// <summary>
        ///     EventHandler triggered after the event SetTaskInProgress
        /// </summary>
        public event EventHandler OnExecute;
        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new EventEntity();
            CopyEntityTo(clone);
            return clone;
        }

        public override void CopyEntityTo(IEntity entity)
        {
            base.CopyEntityTo(entity);
            if (!(entity is EventEntity copy))
            {
                return;
            }

            copy.Step = Step;
            copy.OnExecute = OnExecute;
        }

        public virtual void Schedule(ushort step)
        {
            if (Trigger(step))
            {
                OnExecute?.Invoke(this, null);
            }
        }

        public virtual bool Trigger(ushort step)
        {
            return step == Step;
        }
    }
}