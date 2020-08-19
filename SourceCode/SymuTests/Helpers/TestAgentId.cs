﻿#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
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
        public TestAgentId(ushort id)
        {
            Id = id;
        }
        /// <summary>
        ///     Unique Id of the agent
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        ///     ClassId of the agent
        /// </summary>
        public IClassId ClassId { get; set; }

        public byte Class => ((ClassId?)ClassId)?.Id ?? 0;

        public bool IsNull => Id == 0;
        public bool IsNotNull => Id != 0;

        public TestAgentId(ushort id, byte classId)
        {
            if (classId == 0)
            {
                throw new ArgumentNullException(nameof(classId));
            }

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
                   Id == id.Id;
        }

        public bool Equals(IAgentId agentId)
        {
            return Id == ((TestAgentId)agentId).Id;
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
            return Id < ((TestAgentId)agentId).Id;
        }
    }
}