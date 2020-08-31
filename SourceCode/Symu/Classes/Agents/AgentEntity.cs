#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;

#endregion

namespace Symu.Classes.Agents
{
    /// <summary>
    ///     class for Entity class of the Agent
    /// </summary>
    public class AgentEntity
    {
        public AgentEntity()
        {
        }

        public AgentEntity(IId id, byte classId)
        {
            AgentId = new AgentId(id, classId);
        }

        public AgentEntity(IId id, byte classId, string name) : this(id, classId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public AgentEntity(IId id, byte classId, string name, IAgentId parent) : this(id, classId, name)
        {
            Parent = parent;
        }

        /// <summary>
        ///     The Id of the agent. Each entity must have a unique Id
        ///     FIPA Norm : AID
        /// </summary>
        public IAgentId AgentId { get; set; }

        public string Name { get; set; }
        public IAgentId Parent { get; set; }

        public void CopyTo(AgentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.AgentId = AgentId;
            entity.Name = Name;
            entity.Parent = Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is AgentEntity entity &&
                   AgentId.Equals(entity.AgentId);
        }

        protected bool Equals(AgentEntity other)
        {
            return other != null && AgentId.Equals(other.AgentId);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}