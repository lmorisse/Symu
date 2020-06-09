﻿#region Licence

// Description: Symu - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Environment;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public sealed class GroupAgent : Agent
    {
        public const byte ClassKey = 1;

        public GroupAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey), environment)
        {
        }
    }
}