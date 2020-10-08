#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     SymuEvent helps you schedule random events that happen during the simulation
    /// </summary>
    public class RandomEvent : EventEntity
    {
        private float _ratio;

        public RandomEvent()
        {
        }
        public new static RandomEvent CreateInstance(GraphMetaNetwork metaNetwork)
        {
            return new RandomEvent(metaNetwork);
        }
        public RandomEvent(GraphMetaNetwork metaNetwork) : base(metaNetwork)
        {
        }

        public float Ratio
        {
            get => _ratio;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("Ratio should be between 0 and 1");
                }

                _ratio = value;
            }
        }

        public override bool Trigger(ushort step)
        {
            return Bernoulli.Sample(_ratio);
        }

        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new RandomEvent();
            CopyEntityTo(clone);
            return clone;
        }

        public override void CopyEntityTo(IEntity entity)
        {
            base.CopyEntityTo(entity);
            if (!(entity is RandomEvent copy))
            {
                return;
            }

            copy.Ratio = Ratio;
        }
    }
}