#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common.Interfaces;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA;
using Symu.DNA.Networks;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.DNA.Networks.TwoModesNetworks;
using Symu.Messaging.Templates;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class Database : IResource
    {
        /// <summary>
        ///     the numerical reduction in knowledge if the bit is to be effected by the stochastic forgetting process
        ///     It impacts the KnowledgeBits of the Agent
        ///     It's binary : you find the email or not
        /// </summary>
        private const float ForgettingRate = 1;

        /// <summary>
        ///     Database of the stored information
        /// </summary>
        private readonly AgentExpertise _database = new AgentExpertise();
        private readonly LearningModel _learningModel;
        private readonly ForgettingModel _forgettingModel;

        /// <summary>
        ///     Database unique Identifier
        /// </summary>
        public IId Id { get; }

        public Database(IId id, CommunicationTemplate medium, OrganizationModels organizationModels,
            MetaNetwork metaNetwork)
        {
            if (organizationModels is null)
            {
                throw new ArgumentNullException(nameof(organizationModels));
            }

            Id = id;
            SetCognitiveArchitecture(medium);
            var agentId = new AgentId(Id, 0);
            _learningModel = new LearningModel(agentId, organizationModels, metaNetwork,
                CognitiveArchitecture, organizationModels.Generator);
            _forgettingModel = new ForgettingModel(_database, CognitiveArchitecture, organizationModels);
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

        public void SetCognitiveArchitecture(CommunicationTemplate medium) 
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

        public bool Exists(IId knowledgeId)
        {
            return _database.Contains(knowledgeId);
        }

        public void InitializeKnowledge(IKnowledge iKnowledge, ushort step)
        {
            if (!(iKnowledge is Knowledge knowledge))
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            InitializeKnowledge(knowledge.Id, knowledge.Length, step);
        }

        public void InitializeKnowledge(IId knowledgeId, byte knowledgeLength, ushort step)
        {
            if (Exists(knowledgeId))
            {
                return;
            }

            var agentKnowledge = new AgentKnowledge(knowledgeId, KnowledgeLevel.NoKnowledge, 0, TimeToLive);
            agentKnowledge.InitializeWith0(knowledgeLength, step);
            _database.Add(agentKnowledge);
        }

        /// <summary>
        ///     Store the knowledge from a message of email type
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="maxRateLearnable"></param>
        /// <param name="step"></param>
        public void StoreKnowledge(IId knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
        {
            if (knowledgeId == null || knowledgeId.IsNull || knowledgeBits is null)
            {
                return;
            }

            InitializeKnowledge(knowledgeId, knowledgeBits.Length, step);
            var agentKnowledge = GetKnowledge(knowledgeId);
            _learningModel.Learn(knowledgeBits, maxRateLearnable, agentKnowledge, step);
        }

        /// <summary>
        ///     Store the knowledge bit of the KnowledgeId in the Database
        ///     The Knowledge must have been InitializeKnowledge first
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="knowledgeValue"></param>
        /// <param name="step"></param>
        public void StoreKnowledge(IId knowledgeId, byte knowledgeBit, float knowledgeValue, ushort step)
        {
            if (!Exists(knowledgeId))
            {
                throw new ArgumentNullException("knowledgeId must have been initialized first");
            }

            _learningModel.AgentKnowledgeLearn(GetKnowledge(knowledgeId), knowledgeBit, knowledgeValue, step);
        }

        /// <summary>
        ///     Search information in the database
        /// </summary>
        /// <param name="knowledgeId">the knowledgeId the agent is searching in the database</param>
        /// <param name="knowledgeBit">the knowledgeBit the agent is searching in the database</param>
        /// <param name="minKnowledgeBit">the minKnowledgeBit required to have enough information</param>
        /// <returns>return false if database don't have the information</returns>
        /// <returns>return true if database have enough information</returns>
        public bool SearchKnowledge(IId knowledgeId, byte knowledgeBit, float minKnowledgeBit)
        {
            var agentKnowledge = GetKnowledge(knowledgeId);
            return !(agentKnowledge is null) && agentKnowledge.GetKnowledgeBit(knowledgeBit) > minKnowledgeBit;
        }

        /// <summary>
        ///     Get the sum of all the knowledges
        /// </summary>
        public float GetKnowledgesSum()
        {
            return _database.GetAgentKnowledges<AgentKnowledge>().Sum(l => l.GetKnowledgeSum());
        }

        /// <summary>
        ///     Forget knowledges from the database based on knowledgeBits.LastTouched and timeToLive value.
        ///     if timeToLive == -1, there is no forgetting process
        /// </summary>
        public void ForgettingProcess(ushort step)
        {
            if (TimeToLive < 1)
            {
                return;
            }

            _forgettingModel.ForgettingProcess(ForgettingRate, step);
        }

        public AgentKnowledge GetKnowledge(IId knowledgeId)
        {
            return _database.GetAgentKnowledge<AgentKnowledge>(knowledgeId);
        }
    }
}