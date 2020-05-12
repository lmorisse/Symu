#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Repository.Networks.Databases
{
    /// <summary>
    ///     Database used to store and search information
    /// </summary>
    public class Database
    {
        /// <summary>
        ///     the numerical reduction in knowledge if the bit is to be effected by the stochastic forgetting process
        ///     It impacts the KnowledgeBits of the Agent
        ///     It's binary : you find the email or not
        /// </summary>
        private const float ForgettingRate = 1;

        /// <summary>
        ///     Define the cognitive architecture model of this class
        /// </summary>
        private readonly TasksAndPerformance _cognitive = new TasksAndPerformance();

        /// <summary>
        ///     Database of the stored information
        /// </summary>
        private readonly AgentExpertise _database = new AgentExpertise();

        /// <summary>
        ///     Time to live : information are stored in the database
        ///     But information have a limited lifetime depending on those database
        ///     -1 for unlimited time to live
        ///     Initialized by CommunicationTemplate.TimeToLive
        /// </summary>
        private readonly short _timeToLive;

        public Database(ushort id, TasksAndPerformance cognitive, short timeToLive)
        {
            if (cognitive is null)
            {
                throw new ArgumentNullException(nameof(cognitive));
            }

            Id = id;
            cognitive.CopyTo(_cognitive);
            // the knowledge of the email is entirely stored
            _cognitive.LearningRate = 1;
            _timeToLive = timeToLive;
        }

        /// <summary>
        ///     Database Id
        /// </summary>
        public ushort Id { get; }

        public bool Exists(ushort knowledgeId)
        {
            return _database.Contains(knowledgeId);
        }

        public void InitializeKnowledge(Knowledge knowledge, ushort step)
        {
            if (knowledge == null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            InitializeKnowledge(knowledge.Id, knowledge.Length, step);
        }

        public void InitializeKnowledge(ushort knowledgeId, byte knowledgeLength, ushort step)
        {
            if (Exists(knowledgeId))
            {
                return;
            }

            var agentKnowledge = new AgentKnowledge(knowledgeId, KnowledgeLevel.NoKnowledge, 0, _timeToLive);
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
        public void StoreKnowledge(ushort knowledgeId, Bits knowledgeBits, float maxRateLearnable, ushort step)
        {
            if (knowledgeId == 0 || knowledgeBits is null)
            {
                return;
            }

            InitializeKnowledge(knowledgeId, knowledgeBits.Length, step);
            var agentKnowledge = GetKnowledge(knowledgeId);
            _cognitive.Learn(knowledgeBits, maxRateLearnable, agentKnowledge, step);
        }

        /// <summary>
        ///     Store the knowledge bit of the KnowledgeId in the Database
        ///     The Knowledge must have been InitializeKnowledge first
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="knowledgeValue"></param>
        /// <param name="step"></param>
        public void StoreKnowledge(ushort knowledgeId, byte knowledgeBit, float knowledgeValue, ushort step)
        {
            if (!Exists(knowledgeId))
            {
                throw new ArgumentNullException("knowledgeId must have been initialized first");
            }

            GetKnowledge(knowledgeId).Learn(knowledgeBit, knowledgeValue, step);
        }

        /// <summary>
        ///     Search information in the database
        /// </summary>
        /// <param name="knowledgeId">the knowledgeId the agent is searching in the database</param>
        /// <param name="knowledgeBit">the knowledgeBit the agent is searching in the database</param>
        /// <param name="minKnowledgeBit">the minKnowledgeBit required to have enough information</param>
        /// <returns>return false if database don't have the information</returns>
        /// <returns>return true if database have enough information</returns>
        public bool SearchKnowledge(ushort knowledgeId, byte knowledgeBit, float minKnowledgeBit)
        {
            var agentKnowledge = GetKnowledge(knowledgeId);
            return !(agentKnowledge is null) && agentKnowledge.GetKnowledgeBit(knowledgeBit) > minKnowledgeBit;
        }

        /// <summary>
        ///     Get the sum of all the knowledges
        /// </summary>
        public float GetKnowledgesSum()
        {
            return _database.GetKnowledgesSum();
        }

        /// <summary>
        ///     Forget knowledges from the database based on knowledgeBits.LastTouched and timeToLive value.
        ///     if timeToLive == -1, there is no forgetting process
        /// </summary>
        public void ForgettingProcess(ushort step)
        {
            if (_timeToLive < 1)
            {
                return;
            }

            _database.ForgettingProcess(ForgettingRate, step);
        }

        public AgentKnowledge GetKnowledge(ushort knowledgeId)
        {
            return _database.GetKnowledge(knowledgeId);
        }
    }
}