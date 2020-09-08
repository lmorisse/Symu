#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.Messaging.Templates;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Entity for DataBase class, used to store and search information during the simulation
    /// </summary>
    public class DatabaseEntity : IResource
    {
        public IId Id { get; }
        public DatabaseEntity(IId id)
        {
            Id = id;
        }

        public DatabaseEntity(IId id, CognitiveArchitecture cognitiveArchitecture) : this(id)
        {
            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            CognitiveArchitecture = new CognitiveArchitecture();
            cognitiveArchitecture.CopyTo(CognitiveArchitecture);
        }

        public DatabaseEntity(IId id, CommunicationTemplate medium) : this(id)
        {
            if (medium == null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

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