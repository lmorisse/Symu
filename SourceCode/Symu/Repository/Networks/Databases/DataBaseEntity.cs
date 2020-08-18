#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;

#endregion

namespace Symu.Repository.Networks.Databases
{
    /// <summary>
    ///     Entity for DataBase class, used to store and search information during the simulation
    /// </summary>
    public class DataBaseEntity
    {
        public DataBaseEntity(IAgentId agentId, CognitiveArchitecture cognitiveArchitecture)
        {
            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            AgentId = agentId;
            CognitiveArchitecture = new CognitiveArchitecture();
            cognitiveArchitecture.CopyTo(CognitiveArchitecture);
        }

        public DataBaseEntity(IAgentId agentId, CommunicationTemplate medium)
        {
            if (medium == null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            AgentId = agentId;
            CognitiveArchitecture = new CognitiveArchitecture();
            // Communication
            CognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfBeliefToSend =
                medium.MaximumNumberOfBitsOfBeliefToSend;
            CognitiveArchitecture.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend =
                medium.MaximumNumberOfBitsOfKnowledgeToSend;
            CognitiveArchitecture.MessageContent.MinimumBeliefToSendPerBit =
                medium.MinimumBeliefToSendPerBit;
            CognitiveArchitecture.MessageContent.MinimumKnowledgeToSendPerBit =
                medium.MinimumKnowledgeToSendPerBit;
            CognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfBeliefToSend =
                medium.MinimumNumberOfBitsOfBeliefToSend;
            CognitiveArchitecture.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend =
                medium.MinimumNumberOfBitsOfKnowledgeToSend;
            CognitiveArchitecture.InternalCharacteristics.TimeToLive = medium.TimeToLive;
            // Knowledge
            CognitiveArchitecture.TasksAndPerformance.LearningRate = 1;
            CognitiveArchitecture.InternalCharacteristics.CanLearn = true;
            CognitiveArchitecture.KnowledgeAndBeliefs.HasKnowledge = true;
        }

        /// <summary>
        ///     Database Id
        /// </summary>
        public IAgentId AgentId { get; set; }

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