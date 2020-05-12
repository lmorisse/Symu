#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Classes.Agents
{
    /// <summary>
    ///     class for Entity class of the Agent
    /// </summary>
    public class AgentEntity
    {
        public AgentEntity()
        {
        }

        public AgentEntity(ushort key, byte classKey)
        {
            Id = new AgentId(key, classKey);
        }

        public AgentEntity(ushort key, byte classKey, string name) : this(key, classKey)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public AgentEntity(ushort key, byte classKey, string name, ushort parent) : this(key, classKey, name)
        {
            Parent = parent;
        }

        /// <summary>
        ///     The Id of the agent. Each entity must have a unique Id
        ///     FIPA Norm : AID
        /// </summary>
        public AgentId Id { get; set; }

        public string Name { get; set; }
        public ushort Parent { get; set; }

        public void CopyTo(AgentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id; // new AgentId(Id.Key, Id.ClassKey);
            entity.Name = Name;
            entity.Parent = Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is AgentEntity entity &&
                   Id.Equals(entity.Id);
        }

        protected bool Equals(AgentEntity other)
        {
            return other != null && Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}