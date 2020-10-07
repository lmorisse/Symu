#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Organization;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;
using Symu.Repository.Edges;

#endregion

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class Database : ResourceEntity
    {
        public new const byte Class = SymuYellowPages.Database;
        public new static IClassId ClassId => new ClassId(Class);

        /// <summary>
        ///     the numerical reduction in knowledge if the bit is to be effected by the stochastic forgetting process
        ///     It impacts the KnowledgeBits of the Agent
        ///     It's binary : you find the email or not
        /// </summary>
        private const float ForgettingRate = 1;

        private ForgettingModel _forgettingModel;

        /// <summary>
        ///     Database of the stored information
        /// </summary>
        private LearningModel _learningModel;

        public Database()
        {
            CognitiveArchitecture = new CognitiveArchitecture();
        }

        public Database(GraphMetaNetwork metaNetwork, MainOrganizationModels models, CommunicationTemplate medium,
            byte classId) : base(metaNetwork, classId)
        {
            if (metaNetwork is null)
            {
                throw new ArgumentNullException(nameof(metaNetwork));
            }

            if (models is null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            SetCognitiveArchitecture(medium);
            // There is no random level for database
            _learningModel = new LearningModel(EntityId, models, MetaNetwork.Knowledge, MetaNetwork.ResourceKnowledge,
                CognitiveArchitecture, models.Generator, 0);
            _forgettingModel =
                new ForgettingModel(EntityId, MetaNetwork.ResourceKnowledge, CognitiveArchitecture, models, 0);
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

        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new Database();
            CopyEntityTo(clone);
            return clone;
        }

        public override void CopyEntityTo(IEntity entity)
        {
            base.CopyEntityTo(entity);
            if (!(entity is Database copy))
            {
                return;
            }

            CognitiveArchitecture.CopyTo(copy.CognitiveArchitecture);
            copy._learningModel = _learningModel;
            copy._forgettingModel = _forgettingModel;
        }

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

        public void InitializeKnowledge(IAgentId knowledgeId, ushort step)
        {
            if (ExistsKnowledge(knowledgeId))
            {
                return;
            }

            var knowledge = MetaNetwork.Knowledge.GetEntity<Knowledge>(knowledgeId);
            var resourceKnowledge = new ActorKnowledge(MetaNetwork.ResourceKnowledge, EntityId, knowledgeId, KnowledgeLevel.NoKnowledge, 0, TimeToLive);
            resourceKnowledge.InitializeWith0(knowledge.Length, step);
        }

        /// <summary>
        ///     Store the knowledge from a message of email type
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="maxRateLearnable"></param>
        /// <param name="step"></param>
        public void StoreKnowledge(IAgentId knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
        {
            if (knowledgeId == null || knowledgeId.IsNull || knowledgeBits is null)
            {
                return;
            }

            InitializeKnowledge(knowledgeId, step);
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
        public void StoreKnowledge(IAgentId knowledgeId, byte knowledgeBit, float knowledgeValue, ushort step)
        {
            if (!ExistsKnowledge(knowledgeId))
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
        public bool SearchKnowledge(IAgentId knowledgeId, byte knowledgeBit, float minKnowledgeBit)
        {
            var agentKnowledge = GetKnowledge(knowledgeId);
            return !(agentKnowledge is null) && agentKnowledge.GetKnowledgeBit(knowledgeBit) >= minKnowledgeBit;
        }

        /// <summary>
        ///     Get the sum of all the knowledges
        /// </summary>
        public float GetKnowledgesSum()
        {
            return MetaNetwork.ResourceKnowledge.EdgesFilteredBySource<ActorKnowledge>(EntityId)
                .Sum(l => l.GetKnowledgeSum());
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

        public ActorKnowledge GetKnowledge(IAgentId knowledgeId)
        {
            return MetaNetwork.ResourceKnowledge.Edge<ActorKnowledge>(EntityId, knowledgeId);
        }
    }
}