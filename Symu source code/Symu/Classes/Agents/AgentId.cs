#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Classes.Agents
{
    public struct AgentId
    {
        /// <summary>
        ///     Unique key of the agent
        /// </summary>
        public ushort Key { get; set; }

        /// <summary>
        ///     Class Key of the agent
        /// </summary>
        public byte ClassKey { get; set; }

        public bool IsNull => Key == 0;

        public AgentId(ushort key, byte classKey)
        {
            if (classKey == 0)
            {
                throw new ArgumentNullException(nameof(classKey));
            }

            Key = key;
            ClassKey = classKey;
        }

        /// <summary>
        ///     Don't remove this substitution
        ///     Use Equals and not ContainsKey(agentId) or implement GetHashCode substitution
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is AgentId id &&
                   Key == id.Key; //&&
            //ClassKey == id.ClassKey;
        }

        public bool Equals(AgentId other)
        {
            return Key == other.Key; //&& ClassKey == other.ClassKey;
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public static bool operator ==(AgentId left, AgentId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AgentId left, AgentId right)
        {
            return !(left == right);
        }

        public AgentId Clone()
        {
            return new AgentId(Key, ClassKey);
        }
    }
}