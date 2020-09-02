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
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Networks.OneModeNetworks;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     SymuEvent helps you schedule one shot events that happen during the simulation
    /// </summary>
    public class SymuEvent: IEvent
    {
        public SymuEvent(ushort id)
        {
            Id = new UId(id);
        }
        public SymuEvent(IId id)
        {
            Id = id;
        }
        public ushort Step { get; set; }

        public virtual void Schedule(ushort step)
        {
            if (Trigger(step))
            {
                OnExecute?.Invoke(this, null);
            }
        }

        /// <summary>
        ///     EventHandler triggered after the event SetTaskInProgress
        /// </summary>
        public event EventHandler OnExecute;

        public virtual bool Trigger(ushort step)
        {
            return step == Step;
        }

        /// <summary>
        ///     Unique identifier of the event
        /// </summary>
        public IId Id { get; }
    }
}