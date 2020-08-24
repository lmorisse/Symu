#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal class TestAgentId : IAgentId
    {
        /// <summary>
        ///     Unique Id of the agent
        /// </summary>
        public UId Id { get; set; }

        /// <summary>
        ///     ClassId of the agent
        /// </summary>
        public IClassId ClassId { get; set; }

        public byte Class => ((ClassId?)ClassId)?.Id ?? 0;

        public bool IsNull => Id.IsNull;
        public bool IsNotNull => Id.IsNotNull;

        IId IAgentId.Id => Id;

        public bool Equals(IId id)
        {
            return Id.Equals(id);
        }

        public TestAgentId(ushort id, byte classId)
        {
            Id = new UId(id);
            ClassId = new ClassId(classId);
        }
        public TestAgentId(UId id, byte classId)
        {
            Id = id;
            ClassId = new ClassId(classId);
        }

        /// <summary>
        ///     Don't remove this substitution
        ///     Use Equals and not ContainsKey(agentId) or implement GetHashCode substitution
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TestAgentId id &&
                   Id.Equals(id.Id);
        }

        public bool Equals(IAgentId agentId)
        {
            return agentId is TestAgentId id &&
                   Id.Equals(id.Id);
        }

        public bool Equals(IClassId classId)
        {
            return ClassId.Equals(classId);
        }

        public bool Equals(byte classId)
        {
            return Class == classId;
        }

        /// <summary>
        /// Implement inferior operator
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>true if this is inferior to agentId </returns>
        public bool CompareTo(IAgentId agentId)
        {
            return agentId is TestAgentId agent && Id.Id < agent.Id.Id;
        }
    }
}