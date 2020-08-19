#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Repository.Networks.Resources;

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Define who is using an object and how
    /// </summary>
    public class Resource : IResource
    {

        private readonly AgentId _agentId;
        public Resource(IAgentId agentId)
        {
            _agentId = (AgentId)agentId;
        }
        public Resource(ushort id, byte classId)
        {
            _agentId = new AgentId(id, classId);
        }

        /// <summary>
        ///     The unique agentId of the resource
        /// </summary>
        public IAgentId Id => _agentId;

    }
}