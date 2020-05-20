#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models.CognitiveModel;

#endregion

namespace SymuEngine.Repository.Networks.Databases
{
    /// <summary>
    ///     Entity for DataBase class, used to store and search information during the simulation
    /// </summary>
    public class DataBaseEntity
    {
        public DataBaseEntity(AgentId agentId, CognitiveArchitecture cognitiveArchitecture)
        {
            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            AgentId = agentId;
            CognitiveArchitecture = new CognitiveArchitecture();
            cognitiveArchitecture.CopyTo(CognitiveArchitecture);
        }

        /// <summary>
        ///     Database Id
        /// </summary>
        public AgentId AgentId { get; set; }

        /// <summary>
        ///     Time to live : information are stored in the database
        ///     But information have a limited lifetime depending on those database
        ///     -1 for unlimited time to live
        ///     Initialized by CommunicationTemplate.TimeToLive
        /// </summary>
        public short TimeToLive => CognitiveArchitecture.InternalCharacteristics.TimeToLive;

        /// <summary>
        ///     CognitiveArchitecture of the database
        /// </summary>
        public CognitiveArchitecture CognitiveArchitecture { get; set; }
    }
}