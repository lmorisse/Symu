#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Agent;

#endregion

namespace SymuEngine.Repository.Networks.Link
{
    /// <summary>
    ///     Describe the communication link between two people
    ///     Report to , communicate to
    ///     in the context of a team
    /// </summary>
    /// <example>
    ///     a teammate report to a manager in the context of a team,
    ///     but can communicate to him in the context of a task force
    /// </example>
    public class CommunicationLink : NetworkLink
    {
        /// <summary>
        ///     AgentId1 communicate to AgentId2 in the context of the TeamId
        /// </summary>
        /// <param name="agentId1"></param>
        /// <param name="communication"></param>
        /// <param name="agentId2"></param>
        /// <param name="teamId"></param>
        public CommunicationLink(AgentId agentId1, CommunicationType communication, AgentId agentId2, AgentId teamId) :
            base(agentId1, agentId2)
        {
            Communication = communication;
            TeamId = teamId;
        }

        /// <summary>
        ///     in the context of a team
        /// </summary>
        public AgentId TeamId { get; }

        /// <summary>
        ///     Type of communication
        ///     Report to , communicate to
        /// </summary>
        public CommunicationType Communication { get; }

        public override bool Equals(object obj)
        {
            return obj is CommunicationLink link &&
                   Communication == link.Communication &&
                   AgentId1.Equals(link.AgentId1) &&
                   AgentId2.Equals(link.AgentId2) &&
                   TeamId.Equals(link.TeamId);
        }
    }
}